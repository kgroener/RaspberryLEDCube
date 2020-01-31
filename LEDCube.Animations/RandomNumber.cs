using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDCube.Animations
{
    internal static class RandomNumber
    {
        private static readonly Random _random = new Random();

        public static T GetRandomEnumValue<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException("Type is not an enum");
            }

            var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();

            return GetRandomItem(values);
        }

        public static int GetRandomInteger(int min, int max)
        {
            return _random.Next(min, max + 1);
        }

        public static int GetRandomInteger(int max)
        {
            return GetRandomInteger(0, max);
        }

        public static T GetRandomItem<T>(IEnumerable<T> enumerable)
        {
            var enumerableAsArray = enumerable as T[] ?? enumerable.ToArray();

            var length = enumerableAsArray.Length;

            return enumerableAsArray[GetRandomInteger(length - 1)];
        }

        public static double GetRandomNumber(double min, double max)
        {
            return (GetRandomNumber() * (max - min)) + min;
        }

        public static double GetRandomNumber(double max)
        {
            return GetRandomNumber(0, max);
        }

        public static double GetRandomNumber()
        {
            return _random.NextDouble();
        }
    }
}