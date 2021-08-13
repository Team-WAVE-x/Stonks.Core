using System;

namespace Stonks.Core.Rewrite.Extension
{
    public static class Untility
    {
        public static void InitializeArray<T>(this T[] array) where T : class, new()
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new T();
            }
        }

        public static T[] InitializeArray<T>(int length) where T : class, new()
        {
            T[] array = new T[length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new T();
            }

            return array;
        }
    }
}
