using System;

namespace shared.math
{
    public class Random
    {
        private int _seed=1;

        public Random(int seed)
        {
            SetSeed(seed);
        }

        public void SetSeed(int seed)
        {
            // In original ActionScript, seed must be > 0 and <= 2147483646
            if (seed <= 0 || seed > 2147483646)
            {
                throw new ArgumentException($"Seed must be > 0 and <= 2147483646, got: {seed}");
            }
            _seed = seed;
        }

        public int GetSeed()
        {
            return _seed;
        }

        public uint NextInt()
        {
            return Gen();
        }

        public double NextDouble()
        {
            return Gen() / 2147483647.0;
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
            var seed = (uint)((ulong)_seed * 16807 % 2147483647);
            _seed = (int)seed;
            return seed;
        }
    }
}