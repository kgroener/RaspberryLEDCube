using System;
using System.Collections.Generic;
using System.Text;

namespace LEDCube.Animations.Helpers
{
    internal static class NumberHelper
    {
        public static int Clip(this int value, int min, int max)
        {
            return (value < min ? min : value > max ? max : value);

        }

        public static int Clip(this double value, int min, int max)
        {
            return Clip((int)Math.Round(value), min, max);
        }
    }
}
