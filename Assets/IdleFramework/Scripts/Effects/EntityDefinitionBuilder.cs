﻿using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class EntityDefinitionBuilder : Builder<EntityDefinition>
    {
        private ISet<string> types = new HashSet<string>();
        private string key;
        private string name;
        private Dictionary<string, ValueContainer> costs = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> productionInputs = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> productionOutputs = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> fixedInputs = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> fixedOutputs = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> upkeep = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> minimumProductionOutputs = new Dictionary<string, ValueContainer>();
        private BigDouble startingQuantity = 0;
        private StateMatcher hideEntityMatcher = Never.Instance;
        private StateMatcher disabledWhenMatcher = Never.Instance;
        private StateMatcher availableWhenMatcher = Always.Instance;
        private ISet<ModifierDefinition> modifiers = new HashSet<ModifierDefinition>();
        private ValueContainer quantityCap;
        private bool scaleProduction = true;
        private readonly Dictionary<string, ValueContainer> customProperties = new Dictionary<string, ValueContainer>();

        private bool canBeBought = true;
        private ValueContainer calculatedQuantity;

        public string EntityKey => key;
        public string Name => name;
        public ISet<string> Types => types;
        public BigDouble StartingQuantity => startingQuantity;
        public Dictionary<string, ValueContainer> BaseCosts => costs;
        public Dictionary<string, ValueContainer> BaseProductionInputs => productionInputs;
        public Dictionary<string, ValueContainer> BaseProductionOutputs => productionOutputs;
        public Dictionary<string, ValueContainer> BaseFixedProductionInputs => fixedInputs;
        public Dictionary<string, ValueContainer> BaseFixedProductionOutputs => fixedOutputs;
        public Dictionary<string, ValueContainer> BaseUpkeep => upkeep;
        public Dictionary<string, ValueContainer> BaseMinimumProductionOutputs => minimumProductionOutputs;
        public bool ScaleProductionOnAvailableInputs => scaleProduction;
        public StateMatcher HiddenMatcher => hideEntityMatcher;
        public StateMatcher DisabledMatcher => disabledWhenMatcher;
        public StateMatcher AvailableMatcher => availableWhenMatcher;
        public ISet<ModifierDefinition> Modifiers => modifiers;
        public bool CanBeBought => canBeBought;
        public ValueContainer QuantityCap => quantityCap;

        public Dictionary<string, ValueContainer> CustomStringProperties => customProperties;

        public Dictionary<string, ValueContainer> CustomProperties => customProperties;

        public ValueContainer CalculatedQuantity => calculatedQuantity;

        /*
         * Create a new EntityDefinitionBuilder, for an entity that will have the given key.
         */
        public EntityDefinitionBuilder(string key)
        {
            this.key = key;
        }

        /*
         * Specify the minimum production that the entity will have while active.
         * 
         * Unlike production defined in ProductionOutputs, this minimum production ignores entity quantity.
         */
        public EntityDefinitionBuilder WithFlatMinimumProduction(string entity, BigDouble value)
        {
            minimumProductionOutputs[entity] = Literal.Of(value);
            return this;
        }

        /*
         * Set the displayed name of the entity.
         */
        public EntityDefinitionBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public EntityDefinitionBuilder AvailableWhen(StateMatcher matcher)
        {
            this.availableWhenMatcher = matcher;
            return this;
        }

        /*
         * Add a cost to the entity. These costs are entities which are lost when purchasing new instances of the entity.
         */ 
        public EntityDefinitionBuilder WithCost(string entityRequired, BigDouble quantityRequired)
        {
            costs.Add(entityRequired, Literal.Of(quantityRequired));
            return this;
        }

        /*
         * Add a production input to the entity. These inputs are consumed when the entity generates output and constrains output.
         * 
         * The quantity specified is a fixed amount per entity.
         */
        public EntityDefinitionBuilder WithConsumption(string entity, ValueContainer quantityConsumedPerTick)
        {
            productionInputs[entity] = quantityConsumedPerTick;
            return this;
        }

        public EntityDefinitionBuilder WithConsumption(string entity, BigDouble quantityConsumedPerTick)
        {
            productionInputs[entity] = Literal.Of(quantityConsumedPerTick);
            return this;
        }

        /*
         * Add a production input to the entity. These inputs are consumed when the entity generates output and constrains output.
         * 
         * The quantity specified is a fixed amount per entity.
         */
        public EntityDefinitionBuilder WithConsumption(string entity, BigDouble quantityConsumedPerTick, ValueContainer cap)
        {
            productionInputs[entity] = Min.Of(Literal.Of(quantityConsumedPerTick), cap);
            return this;
        }


        public EntityDefinitionBuilder WithFixedProduction(string entityKey, ValueContainer quantity)
        {
            fixedOutputs[entityKey] = quantity;
            return this;
        }


        public EntityDefinitionBuilder WithFixedConsumption(string entityKey, ValueContainer quantity)
        {
            fixedInputs[entityKey] = quantity;
            return this;
        }

        /*
         * Add a production output to the entity. These outputs are generated by the entity.
         * 
         * The quantity specified is a fixed amount.
         */
        public EntityDefinitionBuilder WithOutput(string entityKey, ValueContainer quantityPerTick)
        {
            productionOutputs[entityKey] = quantityPerTick;
            return this;
        }

        public EntityDefinitionBuilder WithOutput(string entityKey, BigDouble quantityPerTick)
        {
            productionOutputs[entityKey] = Literal.Of(quantityPerTick);
            return this;
        }

        /*
         * Add a production output to the entity. These outputs are generated by the entity.
         * 
         * The quantity specified is based on one or more other properties.
         */
        public EntityDefinitionBuilder WithProduction(string entityKey, ValueContainer quantityPerTick)
        {
            productionOutputs[entityKey] = quantityPerTick;
            return this;
        }

        public EntityDefinitionBuilder WithProduction(string entityKey, BigDouble quantityPerTick)
        {
            productionOutputs[entityKey] = Literal.Of(quantityPerTick);
            return this;
        }

        /*
         * Add a production output to the entity. These outputs are generated by the entity.
         * 
         * The quantity specified is based on some other value and is capped.
         */
        public EntityDefinitionBuilder WithProduction(string entityKey, ValueContainer quantityPerTick, ValueContainer cap)
        {
            return WithProduction(entityKey, Min.Of(quantityPerTick, cap));
        }

        /*
         * Add a type tag to the entity.
         */
        public EntityDefinitionBuilder WithType(string type)
        {
            types.Add(type);
            return this;
        }

        /*
         * Specify the quantity of the entity that is present at the start of the game.
         */
        public EntityDefinitionBuilder WithStartingQuantity(BigDouble startingQuantity)
        {
            this.startingQuantity = startingQuantity;
            return this;
        }
        /*
         * Add an upkeep requirement to the entity. These upkeep values are consumed for each entity each tick and a shortfall causes the entity quantity to be reduced.
         */
        public EntityDefinitionBuilder WithUpkeepRequirement(string entity, BigDouble quantity)
        {
            BaseUpkeep.Add(entity, Literal.Of(quantity));
            return this;
        }

        /*
         * Complete the configuration of the entity, returning the final EntityDefinition.
         */
        public EntityDefinition Build()
        {
            return new EntityDefinition(this);
        }

        /*
         * Returns a configurer for configuring when the entity should be hidden from the user.
         */
        public EntityHideConfigurationBuilder Hidden()
        {
            return new EntityHideConfigurationBuilder(this);
        }

        /*
         * Returns a configurer for configuring when the entity is inactive.
         */
        public EntityDisabledConfigurationBuilder Disabled()
        {
            return new EntityDisabledConfigurationBuilder(this);
        }

        /*
         * Sets the entity as unpurchaseable by the user.
         */
        public EntityDefinitionBuilder Unbuyable()
        {
            this.availableWhenMatcher = Never.Instance;
            return this;
        }

        public EntityDefinitionBuilder WithCustomBooleanProperty(string propertyName, bool value)
        {
            customProperties.Add(propertyName, Literal.Of(true));
            return this;
        }
        public EntityDefinitionBuilder QuantityCappedBy(ValueContainer entityValueContainer)
        {
            quantityCap = entityValueContainer;
            return this;
        }

        public EntityDefinitionBuilder WithCalculatedQuantity(ValueContainer value)
        {
            this.calculatedQuantity = value;
            return this;
        }

        public EntityDefinitionBuilder HiddenAndDisabledWhen(StateMatcher matcher)
        {
            hideEntityMatcher = matcher;
            disabledWhenMatcher = matcher;
            return this;
        }

        /*
         * Class for configuring when the parent entity should be hidden from the user.
         */
        public class EntityHideConfigurationBuilder
        {
            EntityDefinitionBuilder parent;

            public EntityHideConfigurationBuilder(EntityDefinitionBuilder parent)
            {
                this.parent = parent;
            }

            public ISet<string> Types => parent.Types;

            public string EntityKey => parent.EntityKey;

            public string Name => parent.Name;

            public BigDouble StartingQuantity => parent.StartingQuantity;


            public Dictionary<string, ValueContainer> BaseCosts => parent.BaseCosts;

            public Dictionary<string, ValueContainer> BaseUpkeep => parent.BaseUpkeep;

            public Dictionary<string, ValueContainer> BaseProductionInputs => parent.BaseProductionInputs;

            public Dictionary<string, ValueContainer> BaseProductionOutputs => parent.BaseProductionOutputs;

            public bool ScaleProductionOnAvailableInputs => parent.ScaleProductionOnAvailableInputs;

            public StateMatcher HiddenMatcher => parent.HiddenMatcher;

            public StateMatcher DisabledMatcher => parent.DisabledMatcher;
            /*
             * Sets this entity as always hidden.
             */
            public EntityHideConfigurationBuilder Always()
            {
                parent.hideEntityMatcher = IdleFramework.Always.Instance;
                return this;
            }

            /*
             * Return the parent to further configure it.
             */
            public EntityDefinitionBuilder And()
            {
                return parent;
            }
            /*
             * Return the parent to further configure it.
             * /
            public EntityDefinitionBuilder Done()
            {
                return parent;
            }
            /*
             * Add a state matcher to determine when the entity should be hidden.
             */
            public EntityHideConfigurationBuilder When(StateMatcher stateMatcher)
            {
                parent.hideEntityMatcher = stateMatcher;
                return this;
            }
        }

        public class EntityDisabledConfigurationBuilder
        {
            private EntityDefinitionBuilder parent;

            public EntityDisabledConfigurationBuilder(EntityDefinitionBuilder parent)
            {
                this.parent = parent;
            }

            public EntityDefinitionBuilder Done()
            {
                return parent;
            }

            public EntityDisabledConfigurationBuilder When(StateMatcher stateMatcher)
            {
                parent.disabledWhenMatcher = stateMatcher;
                return this;
            }

            public EntityDefinition Build()
            {
                return parent.Build();
            }
        }

        public class EntityModifierDefinitionBuilder
        {
            private EntityDefinitionBuilder parent;

            public EntityModifierDefinitionBuilder(EntityDefinitionBuilder parent)
            {
                this.parent = parent;
            }

            public EntityModifierDefinitionBuilder Active()
            {
                return this;
            }

            public EntityDefinition Build()
            {
                return parent.Build();
            }
        }
    }
}