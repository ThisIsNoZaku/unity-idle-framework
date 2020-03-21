﻿using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public interface LiteralValue : ValueContainer
    {

    }

    public static class Literal
    {
        public static NumberLiteral Of(BigDouble number)
        {
            return new NumberLiteral(number);
        }

        public static BooleanLiteral Of(bool toCompareAgainst)
        {
            return new BooleanLiteral(toCompareAgainst);
        }

        public static StringLiteral Of(string stringValue)
        {
            return new StringLiteral(stringValue);
        } 
    }
}