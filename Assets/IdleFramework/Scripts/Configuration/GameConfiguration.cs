﻿using BreakInfinity;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace IdleFramework
{
    /**
     * Provides configuration for an instance of IdleEngine.
     **/
    public class GameConfiguration
    {
        private ISet<EntityDefinition> entities;
        private ISet<ModifierDefinitionProperties> modifiers;
        private ISet<EngineHookDefinition> hooks;
        private Dictionary<string, BigDouble> universalCustomEntityProperties;

        public ISet<EntityDefinition> Entities { get => entities; set => entities = value; }
        public ISet<ModifierDefinitionProperties> Modifiers { get => modifiers;  }
        public ISet<EngineHookDefinition> Hooks { get => hooks; }
        public Dictionary<string, BigDouble> UniversalCustomEntityProperties { get => universalCustomEntityProperties; }

        public GameConfiguration(ISet<EntityDefinition> entities, ISet<ModifierDefinitionProperties> modifiers, ISet<EngineHookDefinition> hooks, Dictionary<string, BigDouble> universalCustomEntityProperties)
        {
            var entityKeys = new HashSet<string>();
            foreach(var entityDefinition in entities)
            {
                if (!entityKeys.Add(entityDefinition.EntityKey))
                {
                    throw new ArgumentException(String.Format("The key {0} was used multiple times.", entityDefinition.EntityKey));
                }
            }
            this.entities = entities;
            this.modifiers = modifiers;
            this.hooks = hooks;
            this.universalCustomEntityProperties = universalCustomEntityProperties;
        }
    }
}