﻿using BreakInfinity;
using IdleFramework;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class EntityPropertyModifierEffectDefinition : EntityEffectDefinition
    {
        private string subjectKey;
        private string entityProperty;
        private string entitySubProperty;
        private PropertyReference value;
        private EffectType type;

        public string EntityProperty { get => entityProperty; }
        public PropertyReference Value { get => value; }
        public EffectType Type { get => type; }
        public string EntitySubProperty { get => entitySubProperty; }
        public string SubjectKey { get => subjectKey; }

        public EntityPropertyModifierEffectDefinition(string subjectKey, string entityProperty, string entitySubProperty, PropertyReference value, EffectType type)
        {
            this.subjectKey = subjectKey;
            this.entityProperty = entityProperty;
            this.entitySubProperty = entitySubProperty;
            this.value = value;
            this.type = type;
        }

        public EntityPropertyModifierEffectDefinition(string subjectKey, string entityProperty, PropertyReference value, EffectType type): this(subjectKey, entityProperty, "", value, type)
        {

        }

        private BigDouble getBaseValue(GameEntity entity, IdleEngine engine)
        {
            switch(entityProperty)
            {
                case "outputs":
                    if(entity.ProductionOutputs.ContainsKey(entitySubProperty))
                    {
                        return entity.ProductionOutputs[entitySubProperty].Value;
                    } else if (entity.BaseProductionOutputs.ContainsKey(entitySubProperty))
                    {
                        return entity.BaseProductionOutputs[entitySubProperty].GetAsNumber(engine);
                    }
                    return 0;
                case "inputs":
                    if (entity.ProductionInputs.ContainsKey(entitySubProperty))
                    {
                        return entity.ProductionInputs[entitySubProperty].Value;
                    }
                    else if (entity.BaseProductionInputs.ContainsKey(entitySubProperty))
                    {
                        return entity.BaseProductionInputs[entitySubProperty].GetAsNumber(engine);
                    }
                    return 0;
                case "upkeep":
                    if (entity.Upkeep.ContainsKey(entitySubProperty))
                    {
                        return entity.Upkeep[entitySubProperty].Value;
                    }
                    else if (entity.BaseUpkeep.ContainsKey(entitySubProperty))
                    {
                        return entity.BaseUpkeep[entitySubProperty].GetAsNumber(engine);
                    }
                    return 0;
                default:
                    throw new InvalidOperationException();
            }
        }

        private BigDouble calculateValue(BigDouble currentValue, IdleEngine engine)
        {
            switch (type)
            {
                case EffectType.ADD:
                    return currentValue + value.GetAsNumber(engine);
                case EffectType.SUBTRACT:
                    return currentValue - value.GetAsNumber(engine);
                case EffectType.MULTIPLY:
                    return currentValue * value.GetAsNumber(engine);
                default:
                    throw new InvalidOperationException();
            }
        }

        protected bool entityHasPropertyAndSubproperty(GameEntity entity, string entityProperty, string entitySubProperty)
        {
            switch(EntityProperty)
            {
                case "inputs":
                    return entity.BaseProductionInputs.ContainsKey(entitySubProperty) && !entity.BaseProductionInputs[entitySubProperty].Equals(0);
                case "outputs":
                    return entity.BaseProductionOutputs.ContainsKey(entitySubProperty) && !entity.BaseProductionOutputs[entitySubProperty].Equals(0);
                default:
                    throw new InvalidOperationException();
            }
        }

        public override BigDouble CalculateEffect(ModifiableProperty target, IdleEngine engine)
        {
            return calculateValue(target.Value, engine);
        }

        private IEnumerable<GameEntity> findEntity(IdleEngine engine, string entityKey)
        {
            switch(entityKey)
            {
                case "*":
                    return engine.AllEntities.Values;
                default:
                    return new List<GameEntity>() { engine.AllEntities[entityKey] };
            }
        }

        public override IReadOnlyList<ModifiableProperty> GetAffectableProperties(IdleEngine engine)
        {
            IEnumerable<GameEntity> entities = findEntity(engine, subjectKey);
            List<ModifiableProperty> affected = new List<ModifiableProperty>();
            foreach (var entity in entities) {
                switch (entityProperty)
                {
                    case "outputs":
                        affected.Add(entity.ProductionOutputs[entitySubProperty]);
                        break;
                    case "inputs":
                        affected.Add(entity.ProductionInputs[entitySubProperty]);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            return affected.AsReadOnly();
        }

        public override string ToString()
        {
            return String.Format("{0} {1} to property {2} of entity {3}", type, value, entityProperty + (EntitySubProperty != null ? "[" + EntitySubProperty + "]" : ""), subjectKey);
        }
    }
}