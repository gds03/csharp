//using CustomComponents.Algorithms.Collections;
//using CustomComponents.Algorithms.Collections.Generic;
//using System;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.ConsoleApplication
//{
//    class LinkedListProgram
//    {
//        public static void Main(String[] args)
//        {
//            int[] values = new int[] { 1, 2, 3, 4, 6, 9, 12, 15, 20 };
//            LinkedList<int> data = new LinkedList<int>(values);

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

//            // data should be ordered
//            printData();

//            int val;
//            int idx;

//            Console.WriteLine("********* Removing all elements");
//            for (idx = values.Length - 1; idx >= 0; idx--)
//            {
//                val = data.RemoveLast().Data;
//                Console.WriteLine("Expected to remove {0} : got {1}", values[idx], val);
//            }


//            Console.WriteLine("+++++++++ Expect {0} elements in the collection: Current value {1}", 0, data.Count);

//            Console.WriteLine("Expected to not extract anything since collection is empty; extracted value is: {0}", data.RemoveLast());


//            Console.WriteLine("+++++++ Adding elements to the collection at First");

//            for (idx = 0; idx < values.Length; idx++)
//            {
//                data.AddFirst(values[idx]);
//            }


//            // we should see data reversed since we put them on head.
//            printData();

//            for (idx = values.Length - 1; idx >= 0; idx--)
//            {
//                val = data.RemoveFirst().Data; ;
//                Console.WriteLine("Expected to remove {0} : got {1}", values[idx], val);
//            }



//            data = new LinkedList<int>(values);
//            printData();
//            Console.WriteLine("+++++++ collection reverted");
//            data.Reverse();

//            printData();

//            LinkedNode<int> first = data[20];           
//            Console.WriteLine("first node list: {0}", first.List);
//            data.Remove(first);
//            Console.WriteLine("first node should not have any list associated: {0}", first.List);
//            printData();

//            // data.AddAfter(first, 4);
            



//            Console.ReadLine();



//        }

//    }
//}

