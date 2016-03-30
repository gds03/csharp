//using CustomComponents.Algorithms.Collections.Generic;
//using System;
//using System.Diagnostics;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.ConsoleApplication
//{
//    public class DictionaryProgram
//    {
//        const int OPERATIONS = 1000000 * 10;
//        public static void Main(String[] args)
//        {
//            Stopwatch clock = Stopwatch.StartNew();
//            // HashTable<String> h = new HashTable<String>(512 * 512 * 1000);
//            Dictionary<int, int> dict = new Dictionary<int, int>();

//            Console.WriteLine("INSERTING IN COLLECTION");
//            clock.Start();
//            int i = 0;
//            for (; i < OPERATIONS; i++)
//            {
//                dict.Add(i, OPERATIONS - i);
//            }
//            dict.Add(i, i);
//            clock.Stop();

//            Console.WriteLine("Array Growth {0} times and took {1} Miliseconds", dict.GrowthTimes, dict.GrowthOperationMiliseconds);
//            Console.WriteLine("Finished to insert {1} items... Took {0} miliseconds", clock.ElapsedMilliseconds, OPERATIONS);           

//            Console.WriteLine("SEARCHING IN COLLECTION");
//            clock = Stopwatch.StartNew();
//            for ( i = 0; i < OPERATIONS; i++)
//            {
//                int c = dict[i];
//            }
//            clock.Stop();
//            Console.WriteLine("Finished to search {1} items... Took {0} miliseconds", clock.ElapsedMilliseconds, OPERATIONS);

//            Console.WriteLine("REMOVING IN COLLECTION");
//            clock = Stopwatch.StartNew();
//            for ( i = 0; i < OPERATIONS; i++)
//            {
//                dict.Remove(i);
//            }
//            clock.Stop();
//            Console.WriteLine("Finished to remove {1} items... Took {0} miliseconds", clock.ElapsedMilliseconds, OPERATIONS);

//            //IEnumerator<String> it = h.GetEnumerator();
//            //while (it.MoveNext())
//            //{
//            //    Console.WriteLine(it.Current);
//            //}

            
//            Console.ReadLine();


//        }

//    }
//}
