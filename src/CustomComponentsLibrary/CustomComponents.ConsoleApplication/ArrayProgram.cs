//using CustomComponents.Algorithms.Recursion;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.ConsoleApplication
//{
//    public class ArrayTester
//    {
//        public static void Main(String[] args)
//        {
//            LinkedList<int> list = new LinkedList<int>();

//            for (int i = 0; i < 10; i++)
//            {
//                list.AddLast(i);
//            }

//            int[] array = list.ToArray();

//            int[] inverted = ArrayRecursive.MakeArrayOfInverted(array);

//            for (int i = 0, j = 9; i < 10; i++, j--)
//            {
//                Debug.Assert(inverted[i] == j);
//            }

//            Console.WriteLine("TEST PASSED");
//            Console.ReadLine();
//        }
//    }
//}
