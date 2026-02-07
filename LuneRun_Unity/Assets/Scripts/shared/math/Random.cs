using System;

namespace shared.math
{
    public class Random
    {
        private uint _seed=1;

        public Random(uint seed)
        {
            SetSeed(seed);
        }

        public void SetSeed(uint seed)
        {
            // In original ActionScript, seed must be > 0 and <= 2147483646
            if (seed <= 0 || seed >= int.MaxValue)
            {
                throw new ArgumentException($"Seed must be > 0 and <= 2147483646, got: {seed}");
            }
            _seed = seed;
        }

        public uint GetSeed()
        {
            return _seed;
        }

        public uint NextInt()
        {
            return Gen();
        }

        public double NextDouble()
        {
            return 1.0 * Gen() / int.MaxValue;
        }

        public uint NextIntRange(double min, double max)
        {
            min = min - 0.4999;
            max = max + 0.4999;
            return (uint)Math.Round(min + (max - min) * NextDouble());
        }

        public double NextDoubleRange(double min, double max)
        {
            return min + (max - min) * NextDouble();
        }

        private uint Gen()
        {
            _seed = _seed * 16807 % int.MaxValue;
            return _seed;
        }
    }
}