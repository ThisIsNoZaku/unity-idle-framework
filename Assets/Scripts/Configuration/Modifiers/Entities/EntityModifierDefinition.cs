﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.Configuration
{
    /*
     * A modifier which applies to an entity.
     */
    public abstract class EntityModifierDefinition : ModifierDefinition
    {
        public abstract StateMatcher IsActiveMatcher { get; }
    }
}