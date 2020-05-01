﻿using BreakInfinity;
using System.Collections.Generic;

namespace IdleFramework
{    
    public interface ModifierDefinition
    {
        StateMatcher IsActiveMatcher { get; }
    }

    public enum EffectType
    {
        ADD,
        SUBTRACT,
        MULTIPLY
    }
}