//using CustomComponents.Algorithms.Collections.Generic;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.ConsoleApplication
//{
//    class CircularListProgram
//    {
//        public static void Main(String[] args)
//        {
//            int[] values = new int[] { 1, 2, 3, 4, 6, 9, 12, 15, 20 };
//            CircularList<int> data = new CircularList<int>(values);

//            Action printData = () =>
//            {
//                // test enumerators
//                foreach (int v in data)
//                {
//                    Console.WriteLine(v);
//                }
//            };

//            Console.WriteLine("+++++++ collection created");
//            Console.WriteLine("********* listing the collection");

//            printData();

//            int val; bool extracted;
//            int idx = values.Length - 1;

//            Console.WriteLine("********* Removing all elements");
//            while (idx >= 0)
//            {
//                extracted = data.RemoveLast(out val);
//                Console.WriteLine("Expected to remove {0} : got {1}", values[idx], val);

//                idx--;
//            }

//            Console.WriteLine("+++++++++ Expect {0} elements in the collection: Current value {1}", 0, data.Count);

//            extracted = data.RemoveLast(out val);
//            Console.WriteLine("Expected to not extract anything since collectio is empty; extracted flag is: {0}", extracted);


//            Console.WriteLine("+++++++ Adding elements to the collection at First");
//            // add again
//            while (++idx < values.Length)
//            {
//                data.AddLast(values[idx]);
//            }

//            printData();


//            while (--idx >= 0)
//            {
//                extracted = data.RemoveFirst(out val);
//                Console.WriteLine("Expected to remove {0} : got {1}", values[values.Length - 1 - idx], val);
//            }


//            data = new CircularList<int>(values);
//            printData();
//            Console.WriteLine("+++++++ collection reverted");
//            data.Reverse();

//            printData();



//            Console.ReadLine(); 



//        } 

//    }
//}

