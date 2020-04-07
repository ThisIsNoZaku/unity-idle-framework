﻿using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Difference : PropertyReference
    {
        private ValueContainer left;
        private ValueContainer right;

        public Difference(ValueContainer left, ValueContainer right)
        {
            this.left = left;
            this.right = right;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            var leftHand = left.GetAsNumber(engine);
            var rightHand = right.GetAsNumber(engine);
            return left.GetAsNumber(engine) - right.GetAsNumber(engine);
        }

        public static Difference Between(ValueContainer left, ValueContainer right)
        {
            return new Difference(left, right);
        }

        public bool GetAsBoolean(IdleEngine toCheck)
        {
            return !BigDouble.Zero.Equals(GetAsNumber(toCheck));
        }

        public string GetAsString(IdleEngine engine)
        {
            return GetAsNumber(engine).ToString();
        }

        object ValueContainer.RawValue(IdleEngine engine)
        {
            return GetAsNumber(engine);
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }
    }
}