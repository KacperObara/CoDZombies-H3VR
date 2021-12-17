#if H3VR_IMPORTED

using System;
using System.Collections.Generic;
namespace CustomScripts
{
    public static class Extensions
    {
        // Own implementation of shuffle, because Unity cannot load IListExtensions for some reason
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}

#endif