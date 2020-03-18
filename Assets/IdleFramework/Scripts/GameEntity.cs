﻿using BreakInfinity;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace IdleFramework
{
    public class GameEntity : Modifier, EntityDefinitionProperties
    {
        private readonly IdleEngine engine;
        private BigDouble quantityCap;
        private BigDouble _quantity = 0;
        private BigDouble _progress = 0;
        private readonly EntityDefinition definition;
        private readonly ModifiableProperty quantityChangePerSecond;
        
        private readonly Dictionary<string, IdleFramework.ModifiableProperty> requirements = new Dictionary<string, IdleFramework.ModifiableProperty>();
        private readonly Dictionary<string, IdleFramework.ModifiableProperty> costs = new Dictionary<string, IdleFramework.ModifiableProperty>();
        private readonly Dictionary<string, IdleFramework.ModifiableProperty> productionInputs = new Dictionary<string, IdleFramework.ModifiableProperty>();
        private readonly Dictionary<string, IdleFramework.ModifiableProperty> productionOutputs = new Dictionary<string, IdleFramework.ModifiableProperty>();
        private readonly Dictionary<string, IdleFramework.ModifiableProperty> upkeep = new Dictionary<string, IdleFramework.ModifiableProperty>();
        private readonly Dictionary<string, IdleFramework.ModifiableProperty> minimumProduction = new Dictionary<string, IdleFramework.ModifiableProperty>();
        private readonly Dictionary<string, IdleFramework.ModifiableProperty> customProperties = new Dictionary<string, IdleFramework.ModifiableProperty>();

        public string EntityKey => definition.EntityKey;
        public string Name => definition.Name;
        public BigDouble StartingQuantity => definition.StartingQuantity;

        public bool IsEnabled => !ShouldBeDisabled(engine);

        public Dictionary<string, PropertyReference> BaseRequirements => definition.BaseRequirements;
        public Dictionary<string, PropertyReference> BaseCosts => definition.BaseCosts;
        public Dictionary<string, PropertyReference> BaseProductionInputs => definition.BaseProductionInputs;
        public Dictionary<string, PropertyReference> BaseProductionOutputs => definition.BaseProductionOutputs;
        public Dictionary<string, PropertyReference> BaseUpkeep => definition.BaseUpkeep;
        public BigDouble Quantity {
            get {
                var actualQuantity = _quantity;
                var cap = QuantityCap != null ? QuantityCap.Get(engine) : _quantity;
                return BigDouble.Min(actualQuantity, cap);
           }
        }
        public BigDouble Progress => _progress;
        public ISet<string> Types => definition.Types;
        public bool ScaleProductionOnAvailableInputs => definition.ScaleProductionOnAvailableInputs;
        public StateMatcher HiddenMatcher => definition.HiddenMatcher;
        public StateMatcher DisabledMatcher => definition.DisabledMatcher;
        public PropertyReference QuantityCap => definition.QuantityCap;

        /*
         * The quantities of entities which are required when trying to buy this entity.
         */
        public Dictionary<string, IdleFramework.ModifiableProperty> Requirements => requirements;
        /*
         * The entities and quantities which are consumed to buy this entity.
         */
        public Dictionary<string, IdleFramework.ModifiableProperty> Costs => costs;
        /*
         * The entities and quantities which are consumed each tick by this entity and if a shortfall of these requirements causes the loss of this entity.
         */
        public Dictionary<string, IdleFramework.ModifiableProperty> Upkeep => upkeep;
        /*
         * The entities and quantities which are consumed by this entity as inputs to their production.
         */
        public Dictionary<string, IdleFramework.ModifiableProperty> ProductionInputs => productionInputs;
        /*
         * The entities and quantities that this entity produces each tick, and the entities and quantities that are required to produce without being consumed and entities and quantities which are consumed to produce.
         */
        public Dictionary<string, IdleFramework.ModifiableProperty> ProductionOutputs => productionOutputs;
        public Dictionary<string, IdleFramework.ModifiableProperty> MinimumProductionOutputs => minimumProduction;
        public ISet<ModifierDefinition> Modifiers => ((EntityDefinitionProperties)definition).Modifiers;
        public Dictionary<string, PropertyReference> BaseMinimumProductionOutputs => definition.BaseMinimumProductionOutputs;
        public bool CanBeBought => definition.CanBeBought;
        public BigDouble RealQuantity => _quantity;

        public Dictionary<string, PropertyReference> CustomProperties => definition.CustomProperties;

        public ModifiableProperty QuantityChangePerSecond => quantityChangePerSecond;

        public GameEntity(EntityDefinition definition, IdleEngine engine): base(definition, new HashSet<Effect>())
        {
            this.definition = definition;
            _quantity = definition.StartingQuantity;
            foreach(var property in definition.CustomProperties)
            {
                customProperties.Add(property.Key, new IdleFramework.ModifiableProperty(property.Key, 0, engine));
            }
            this.engine = engine;
            quantityChangePerSecond = new ModifiableProperty("production-per-second", 0, engine);
        }

        public void Buy(BigDouble quantityToBuy, bool buyAllOrNone)
        {
            engine.BuyEntity(this, quantityToBuy, buyAllOrNone);
        }

        public void Buy(BigDouble quantityToBuy)
        {
            Buy(quantityToBuy, false);
        }

        public bool RequirementAreMet()
        {
            var requirementsMet = true;
            foreach(var requirement in Requirements)
            {
                requirementsMet = engine.AllEntities[requirement.Key].Quantity >= requirement.Value.Value;
            }
            return requirementsMet;
        }

        internal void AddModifier(ModifierDefinition modifier)
        {
            throw new NotImplementedException();
        }

        /**
         * Determine the number of entities which are able to produce.
         */
        public BigDouble DetermineProduction()
        {
            var quantityAbleToProduce = Quantity;
            foreach (var requirement in ProductionInputs)
            {
                var quantityWithSufficientInputs = BigDouble.Min(engine.AllEntities[requirement.Key].Quantity / requirement.Value.Value, this.Quantity);
                if(!ScaleProductionOnAvailableInputs)
                {
                    quantityAbleToProduce = 0;
                    break;
                }
            }
            return quantityAbleToProduce;
        }

        public void ChangeQuantity(BigDouble changeBy)
        {
            _quantity += changeBy;
            _quantity = BigDouble.Min(_quantity, definition.QuantityCap.Get(engine));
        }

        public void SetQuantity(BigDouble newQuantity)
        {
            _quantity = newQuantity;
            _quantity = BigDouble.Min(_quantity, definition.QuantityCap.Get(engine));
        }

        public void ChangeProgress(BigDouble changeBy)
        {
            _progress += changeBy;
            if (_progress >= 1)
            {
                _progress = 0;
                _quantity += 1;
            }
            _quantity = BigDouble.Min(_quantity, definition.QuantityCap.Get(engine));
        }

        public void SetProgress(int newProgress)
        {
            _progress = newProgress % 1000;
        }

        public bool ShouldBeHidden(IdleEngine engine)
        {
            return definition.HiddenMatcher.Matches(engine);
        }

        public bool ShouldBeDisabled(IdleEngine engine)
        {
            return definition.DisabledMatcher.Matches(engine);
        }

        public bool HasCustomProperty(string customProperty)
        {
            return customProperties.ContainsKey(customProperty);
        }

        public override string ToString()
        {
            return String.Format("GameEntity({0}) x {1}", this.Name, this.Quantity);
        }

        public ModifierEffect AsModifierEffectFor(IdleEngine engine, string subject, string property)
        {
            string modifierKey = String.Format("{0}-{1}-{2}-{3}", EntityKey, subject, "production", property);
            switch(property)
            {
                case "production":
                    return new ModifierEffect(
                        this,
                        new Effect(new EntityPropertyModifierEffectDefinition(subject, "production", new EntityPropertyReference(this.EntityKey, "outputs", subject), EffectType.ADD), engine)
                        );
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public class ModifiableProperty
    {
        private readonly string propertyName;
        private readonly BigDouble baseValue;
        private BigDouble calculatedValue;
        private List<ModifierEffect> appliedModifiers = new List<ModifierEffect>();
        private IdleEngine engine;

        public ModifiableProperty(string propertyName, BigDouble quantity, IdleEngine engine, params ModifierEffect[] initialModifiers)
        {
            this.propertyName = propertyName;
            this.engine = engine;
            this.baseValue = quantity != null ? quantity : default(BigDouble);
            this.calculatedValue = this.baseValue;
            appliedModifiers.AddRange(initialModifiers);
            calculateValue();
        }

        private void calculateValue()
        {
            calculatedValue = baseValue;
            foreach(var modifierEffect in appliedModifiers)
            {
                calculatedValue = modifierEffect.effect.CalculateEffect(this);
            }
        }

        public BigDouble Value => calculatedValue;

        public IReadOnlyList<ModifierEffect> AppliedModifiers => appliedModifiers.AsReadOnly();

        public void AddModifierEffect(ModifierEffect modifierEffect)
        {
            appliedModifiers.Add(modifierEffect);
            calculateValue();
        }

        public void RemoveModifierEFfect(ModifierEffect modifierAndEffect)
        {
            appliedModifiers.Remove(modifierAndEffect);
            calculateValue();
        }

        public static implicit operator BigDouble(ModifiableProperty gep) => gep.calculatedValue;

        public static bool operator ==(ModifiableProperty left, ModifiableProperty right) => left.calculatedValue.Equals(right.calculatedValue);
        public static bool operator ==(ModifiableProperty left, BigDouble right) => left.calculatedValue.Equals(right);

        public static bool operator !=(ModifiableProperty left, ModifiableProperty right) => !left.calculatedValue.Equals(right.calculatedValue);
        public static bool operator !=(ModifiableProperty left, BigDouble right) => !left.calculatedValue.Equals(right);

        public static BigDouble operator -(ModifiableProperty operand) => -operand.calculatedValue;
        public static BigDouble operator +(ModifiableProperty left, ModifiableProperty right) => left.calculatedValue + right.calculatedValue;

        public static BigDouble operator /(ModifiableProperty left, ModifiableProperty right) => left.calculatedValue / right.calculatedValue;
        public static BigDouble operator /(BigDouble left, ModifiableProperty right) => left / right.calculatedValue;
        public static BigDouble operator /(ModifiableProperty left, BigDouble right) => left.calculatedValue / right;

        public override bool Equals(object obj)
        {
            return obj is ModifiableProperty property &&
                   baseValue.Equals(property.baseValue) &&
                   EqualityComparer<List<ModifierEffect>>.Default.Equals(appliedModifiers, property.appliedModifiers);
        }

        public override int GetHashCode()
        {
            var hashCode = 1254980278;
            hashCode = hashCode * -1521134295 + baseValue.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<ModifierEffect>>.Default.GetHashCode(appliedModifiers);
            return hashCode;
        }

        public override string ToString()
        {
            return String.Format("Property({0}) x {1}", propertyName, calculatedValue);
        }
    }

    public struct ModifierEffect
    {
        public readonly Modifier modifier;
        public readonly Effect effect;

        public ModifierEffect(Modifier modifier, Effect effect)
        {
            this.modifier = modifier;
            this.effect = effect;
        }
    }
}