using CustomComponents.Algorithms.Collections.Generic;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.ConsoleApplication
{
    public class HashTableProgram
    {
        const int OPERATIONS = 1000000 * 10;
        public static void Main(String[] args)
        {
            Stopwatch clock = new Stopwatch();
            // HashTable<String> h = new HashTable<String>(512 * 512 * 1000);
            HashTable<int> h = new HashTable<int>();

            Console.WriteLine("INSERTING IN COLLECTION");
            clock.Start();
            for (int i = 0; i < OPERATIONS; i++)
            {
                h.Add(i);
            }
            clock.Stop();
            Console.WriteLine("Finished to insert... Took {0} miliseconds", clock.ElapsedMilliseconds);
            Console.WriteLine("Array Growth {0} times and took {1} Miliseconds", h.GrowthTimes, h.GrowthOperationMiliseconds);

            Console.WriteLine("SEARCHING IN COLLECTION");
            clock.Restart();
            for (int i = 0; i < OPERATIONS; i++)
            {
                bool c = h.Search(i);
            }
            clock.Stop();
            Console.WriteLine("Finished to search... Took {0} miliseconds", clock.ElapsedMilliseconds);

            Console.WriteLine("REMOVING IN COLLECTION");
            clock.Restart();
            for (int i = 0; i < OPERATIONS; i++)
            {
                h.Remove(i);
            }
            clock.Stop();
            Console.WriteLine("Finished to remove... Took {0} miliseconds", clock.ElapsedMilliseconds);

            //IEnumerator<String> it = h.GetEnumerator();
            //while (it.MoveNext())
            //{
            //    Console.WriteLine(it.Current);
            //}

            
            Console.ReadLine();


        }

    }
}
