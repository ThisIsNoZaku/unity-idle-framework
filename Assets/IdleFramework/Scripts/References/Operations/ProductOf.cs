﻿using BreakInfinity;
namespace IdleFramework
{
    public class Product: PropertyReference
    {
        private ValueContainer left;
        private ValueContainer right;

        private Product(ValueContainer left, ValueContainer right)
        {
            this.left = left;
            this.right = right;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            var leftValue = left.GetAsNumber(engine);
            var rightValue = right.GetAsNumber(engine);
            return  leftValue * rightValue;
        }

        public static Product Of(ValueContainer left, ValueContainer right)
        {
            return new Product(left, right);
        }

        public bool GetAsBoolean(IdleEngine toCheck)
        {
            return BigDouble.Zero.Equals(GetAsNumber(toCheck));
        }

        public string GetAsString(IdleEngine engine)
        {
            return GetAsNumber(engine).ToString();
        }

        public object RawValue(IdleEngine engine)
        {
            return GetAsNumber(engine);
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }
    }
}