﻿using System;

namespace IdleFramework
{
    public interface EngineHookDefinitionProperties<in I, out O>
    {
        EngineHookEvent Action { get; }
        string Actor { get; }
        string Subject { get; }
        Func<I, O> Function { get; }
    }
}