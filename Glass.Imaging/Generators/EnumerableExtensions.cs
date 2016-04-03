namespace Glass.Imaging.Generators
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static IEnumerable<int> Range(int start, int end, Func<int, int> step)
        {
            //check parameters
            while (start <= end)
            {
                yield return start;
                start = step(start);
            }
        }
    }
}