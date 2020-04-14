﻿using System;

namespace IdleFramework
{
    public class MissingEntityException : Exception
    {
        private readonly string entityKey;

        public MissingEntityException(string entityKey) : base(string.Format("Entity {0} does not exist.", entityKey))
        {

        }
    }
}