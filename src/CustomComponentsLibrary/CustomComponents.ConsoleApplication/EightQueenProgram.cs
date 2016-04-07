//using CustomComponents.Algorithms.Recursion.Games.EightQueen;
//using CustomComponents.Algorithms.Recursion.Games.Maze;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.ConsoleApplication
//{
//    public class EightQueenTester
//    {
//        public static void Main(String[] args)
//        {
//            EightQueenV2 queenGame = new EightQueenV2(8);

//            var solutions = queenGame.Generate();

//            int total = 0;
//            foreach (var lineXsolutions in solutions)
//            {
//                Console.WriteLine("solutions for line {0}", lineXsolutions.Count());
//                int c = 0;
//                foreach (var solution in lineXsolutions)
//                {
//                    for (int i = 0; i < solution.Length; i++)
//                    {
//                        Console.WriteLine("(Y: {0}, X:{1})", i, solution[i]);
//                    }
//                    Console.WriteLine("**********************");
//                    c++;
//                    total++;
//                }
//                Console.WriteLine("Solutions found -> {0}", c);
//                Console.WriteLine("Press enter to procede");
//                Console.ReadLine();
//            }




//            Console.WriteLine("Total solutions found: {0}", total);

//            Console.ReadLine();
//        }
//    }
//}
