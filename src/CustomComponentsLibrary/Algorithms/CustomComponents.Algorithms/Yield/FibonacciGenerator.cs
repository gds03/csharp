using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Yield
{
    public static class FibonacciGenerator
    {
        public static IEnumerable<ulong> Generate(ulong n)
        {
            if (n < 1) yield break;
            yield return 1;
            ulong prev = 0;
            ulong next = 1;

            for (ulong i = 1; i < n; i++)
            {
                ulong sum = prev + next;
                prev = next;
                next = sum;
                yield return sum;
            }
        }
    }
}
