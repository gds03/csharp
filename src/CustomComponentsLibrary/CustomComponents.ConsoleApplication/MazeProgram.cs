//using CustomComponents.Algorithms.Recursion.Games.Maze;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.ConsoleApplication
//{
//    public class MazeTester
//    {
//        public static void Main(String[] args)
//        {
//            String[][] board = new String[][]
//            {
//                new []{"1","1","1","1","1","1","1","1","1","1","1"},
//                new []{"1","0","0","0","0","0","1","0","0","0","1"},
//                new []{"1","0","1","0","0","0","1","0","1","0","1"},
//                new []{"E","0","1","0","0","0","0","0","1","0","1"},
//                new []{"1","0","1","1","1","1","1","0","1","0","1"},
//                new []{"1","0","1","0","1","0","0","0","1","0","1"},		
//                new []{"1","0","0","0","1","0","1","0","0","0","1"},
//                new []{"1","1","1","1","1","0","1","0","0","0","1"},
//                new []{"1","0","1","M","1","0","1","0","0","0","1"},
//                new []{"1","0","0","0","0","0","1","0","0","0","1"},
//                new []{"1","1","1","1","1","1","1","1","1","1","1"},	
//            };

//            bool[][] boolboard = new bool[board.Length][];
//            int maxSize = board.Max(x => x.Length);
//            Debug.Assert(board.All(x => x.Length == maxSize));

//            for (int i = 0; i < board.Length; i++)
//            {
//                boolboard[i] = new bool[maxSize];
//                for (int j = 0; j < board[i].Length; j++)
//                {
//                    boolboard[i][j] = new[] { "0", "E", "M" }.Any(x => x == board[i][j]) ? false : true;
//                }
//            }

//            Maze m = new Maze(boolboard, new Point(8, 3), new Point(3, 0));
//            m.DiscoverMaze();
//            Console.ReadLine();
//        }
//    }
//}
