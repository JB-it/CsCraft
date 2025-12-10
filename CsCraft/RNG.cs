using System;
using System.Collections.Generic;
using System.Text;

namespace CsCraft
{
    internal class RNG
    {
        private Random random;
        public RNG(int seed)
        {
            random = new Random(seed);
        }
        public int NextInt(int min, int max)
        {
            return random.Next(min, max);
        }
        public float NextFloat()
        {
            return (float)random.NextDouble();
        }
    }
}
