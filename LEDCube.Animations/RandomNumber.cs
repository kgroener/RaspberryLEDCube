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

        public static int GetRandomNumber(int min, int max)
        {
            return _random.Next(min, max + 1);
        }

        public static int GetRandomNumber(int max)
        {
            return GetRandomNumber(0, max);
        }

        public static T GetRandomItem<T>(IEnumerable<T> enumerable)
        {
            var enumerableAsArray = enumerable as T[] ?? enumerable.ToArray();

            var length = enumerableAsArray.Length;

            return enumerableAsArray[GetRandomNumber(length - 1)];
        }

        public static T GetRandomEnumValue<T>() where T : struct, IConvertible
        { 
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException("Type is not an enum");
            }

            var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();

            return GetRandomItem(values);
        }
    }
}
