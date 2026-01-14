using System;

namespace shared.math
{
    public class Random
    {
        private uint _seed;

        public Random(uint seed)
        {
            SetSeed(seed);
        }

        public void SetSeed(uint seed)
        {
            // Note: In original ActionScript, seed must be >0 and <= 2147483646
            // We'll just accept any positive seed, but for compatibility we can keep the range
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
            _seed = (uint)((ulong)_seed * 16807 % 2147483647);
            return _seed;
        }
    }
}