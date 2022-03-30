using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTP.Utilities
{
    [Serializable]
    public struct Tupple<T>
    {
        public T min;
        public T max;

        private Tupple((T min, T max) pair)
        {
            min = pair.min;
            max = pair.max;
        }

        public static implicit operator Tupple<T>((T min, T max) pair)
        {
            return new Tupple<T>(pair);
        }
    }
}

