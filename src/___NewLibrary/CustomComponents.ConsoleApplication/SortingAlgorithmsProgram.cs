using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Sorting
{
    class SortingAlgorithmsProgram
    {
        const UInt64 N = (1 << 25);
        static void F(ref uint a, ref uint b) { (a) ^= (b); (b) ^= (a); (a) ^= (b); }

        static void Main(string[] args)
        {
            Console.WriteLine("Please wait...");
            System.Diagnostics.Stopwatch t1 = new System.Diagnostics.Stopwatch();

            t1.Start();
            Test();
            t1.Stop();

            Console.WriteLine("Elapsed time: {0}s", t1.ElapsedMilliseconds / 1000);
            Console.ReadLine();
        }

        private static void Test()
        {
            // UInt32[] d = new UInt32[] { 38, 27, 43, 3, 9, 82, 10 };
            UInt32[] d = new UInt32[N];

            for (UInt64 i = 1; i < N; i++)
            {
                d[i] = d[i - 1] * 1919199 + 1;
            }

            // Sort.Merge(d);
            // Sort.MergeParallel(d);
            Sort.MergeSortIterative(d, 0, d.Length - 1);

            UInt64 h = 0;
            for (UInt64 i = 0; i < N; i++)
                h = h * 13 + d[i];

            h ^= 0x8973ADEA5D5765D5;

            Console.WriteLine("Welcome to {0}ta!", VarAddressToString(h));
        }

        private unsafe static string VarAddressToString(ulong source)
        {
            ulong* p = &source;
            string s = p->ToString("x");

            ASCIIEncoding ascii = new ASCIIEncoding();

            byte[] ba = new byte[(s.Length / 2)];

            for (int z = 1; z < s.Length / 2 + 1; z++)
            {
                ba[z - 1] = Convert.ToByte(s.Substring(s.Length - (2 * z), 2), 16);
            }

            return Encoding.ASCII.GetString(ba);
        }
    }
}
