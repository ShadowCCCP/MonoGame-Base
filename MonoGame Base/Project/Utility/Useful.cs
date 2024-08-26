using System;

namespace MonoGame_Base.Project.Utility
{
    public static class Useful
    {
        private static Random _rand = new Random();

        // Get a random float...
        public static float NextFloat(float minValue, float maxValue)
        {
            return (float)(minValue + _rand.NextDouble() * (maxValue - minValue));
        }
    }
}
