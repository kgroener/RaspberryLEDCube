using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDCube.Animations
{
    internal static class RandomNumber
    {
        private static Random _random = new Random();

        public static int GetRandomNumber(int min, int max)
        {
            return _random.Next(min, max + 1);
        }

        public static int GetRandomNumber(int max)
        {
            return GetRandomNumber(0, max);
        }
    }
}
