using CustomComponents.Algorithms.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.ConsoleApplication
{
    public class HashTableProgram
    {
        public static void Main(String[] args)
        {
            HashTable<String> h = new HashTable<String>();

            for (int i = 0; i < 10000; i++)
            {
                h.Add(i.ToString());
            }

            IEnumerator<String> it = h.GetEnumerator();
            while (it.MoveNext())
            {
                Console.WriteLine(it.Current);
            }

            Console.ReadLine();


        }

    }
}
