using System;

namespace Vector.Share.Providers.Random
{
    public class Random : IRandom
    {
        private readonly System.Random _rng;

        public Random()
        {
            _rng = new System.Random(Environment.TickCount);
        }

        public int Next() => _rng.Next();

        public int Next(int maxValue) => _rng.Next(maxValue);

        public int Next(int minValue, int maxValue) => _rng.Next(minValue, maxValue);
    }
}
