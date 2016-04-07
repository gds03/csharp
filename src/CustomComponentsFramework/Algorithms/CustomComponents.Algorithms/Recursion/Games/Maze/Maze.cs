using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Recursion.Games.Maze
{


    /// <summary>
    ///     Represent the next problem in computer science:
    ///     You have a maze with a entry and exit. The entry can be somewhere in the maze and exit can be random too.
    ///     Do a piece of code that given a maze, entry and exit, finds the way(s) out.
    /// </summary>
    public class Maze
    {
        private enum Data 
        {
            FLOOR = 0,
            WALL = 1,
            PATH_RUN = 2,
            WAY_OUT = 3
        }

        

        public const int MIN_MAZESIZE = 8;



        private readonly Data[][] m_board;
        private readonly Point m_entry, m_exit;


        private Stack<Point> m_discoverStack = null;
        private int totalIterations;

        // we can represent a maze as array of arrays.

        public Maze(bool[][] mazeMatrix, Point entry, Point exit )
        {
            // Invariants checking
            if (mazeMatrix == null)
                throw new ArgumentNullException("mazeMatrix");

            if (mazeMatrix.Length < MIN_MAZESIZE)
                throw new ArgumentException(String.Concat("mazeMatrix must be greater than ", MIN_MAZESIZE - 1, " size"));

            var maxArrayLength = mazeMatrix.Max(x => x.Length);
            if (!mazeMatrix.All(x => x.Length == maxArrayLength))
                throw new InvalidOperationException("Inner arrays must be equal in size.");

            if (entry.IsInvalid())
                throw new ArgumentException("entry");

            if (exit.IsInvalid())
                throw new ArgumentException("exit");

            // check valid entry and exit
            if (mazeMatrix[entry.X][entry.Y])
                throw new InvalidOperationException("entry is not valid because is a wall");

            if (mazeMatrix[exit.X][exit.Y])
                throw new InvalidOperationException("exit is not valid because is a wall");

            m_entry = entry;
            m_exit = exit;

            Debug.WriteLine("Creating board in memory");

            // Transform current maze into our Data
            m_board = new Data[mazeMatrix.Length][];

            for (int i = 0; i < mazeMatrix.Length; i++)
            {
                m_board[i] = new Data[maxArrayLength];
                for (int j = 0; j < maxArrayLength; j++)
                {
                    m_board[i][j] = TransformValue(mazeMatrix[i][j]);
                }
            }

            Debug.WriteLine("Board is filled with internal data");
        }

        public IEnumerable<Point> DiscoverMaze()
        {
            List<Point> pathWayOut = new List<Point>();

            // Copy a new instance of Data to work with.
            var tempBoard = MakeAcopy();

            // start to put Points from the exit to the entry - recursive call "remembers" the steps.
            m_discoverStack = new Stack<Point>();

            // call recursive code and start to discover.
            if (DiscoverMazeR(tempBoard, m_entry))
            {
                // m_discoverStack has all the Path
                printWayout(tempBoard);

                printBoard(tempBoard);

                return m_discoverStack.ToList();
            }

            return new Stack<Point>();
        }

        









        #region Private Methods

        private void printWayout(Data[][] board)
        {
            Console.WriteLine("Executed all {0} steps.", totalIterations);
            Console.WriteLine("Path found with {0} steps... ", m_discoverStack.Count);
            Console.WriteLine("");
            Console.WriteLine("Path from the entry to the exit");

            int count = 0;
            Stack<Point> copy = new Stack<Point>(m_discoverStack);

            while (copy.Count > 0)
            {
                count++;
                Point p = copy.Pop();
                Console.Write(p + (copy.Count > 0 ? "# " : ""));

                if (count % 4 == 0)
                    Console.WriteLine();
            }
        }



        private bool DiscoverMazeR(Data[][] board, Point p)
        {
            // trace the path.

            board[p.X][p.Y] = Data.PATH_RUN;
            totalIterations++;

            if (p == m_exit) {
                m_discoverStack.Push(p);            // filled from end to the start
                return true;
            }
            

            if (GoUp(board, p))
            {
                if (DiscoverMazeR(board, new Point(p).SetX(p.X - 1)))
                {
                    m_discoverStack.Push(p);
                    return true;
                }
            }

            if (GoDown(board, p))
            {
                if (DiscoverMazeR(board, new Point(p).SetX(p.X + 1)))
                {
                    m_discoverStack.Push(p);
                    return true;
                }
            }

            if (GoRight(board, p))
            {
                if (DiscoverMazeR(board, new Point(p).SetY(p.Y + 1)))
                {
                    m_discoverStack.Push(p);
                    return true;
                }
            }

            if (GoLeft(board, p))
            {
                if (DiscoverMazeR(board, new Point(p).SetY(p.Y - 1)))
                {
                    m_discoverStack.Push(p);
                    return true;
                }
            }

            return false;
        }

        private void printBoard(Data[][] board)
        {
            Console.WriteLine();
            //update with points
            while (m_discoverStack.Count > 0)
            {
                Point p = m_discoverStack.Pop();
                board[p.X][p.Y] = Data.WAY_OUT;
            }

            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < m_board[0].Length; j++)
                {
                    Console.Write(TransformValue(board[i][j]));
                }
                Console.WriteLine();
            }
        }

        private bool GoLeft(Data[][] board, Point p)
        {
            p.DecY();       // not affect outside code because is copied to this method stack - valuetype
            if (p.Y < 0) return false;
            if (new Data[] { Data.PATH_RUN, Data.WALL }.Any(x => x == board[p.X][p.Y])) return false;
            return true;
        }

        private bool GoRight(Data[][] board, Point p)
        {
            p.IncY();       // not affect outside code because is copied to this method stack - valuetype
            if (p.Y >= board[p.X].Length) return false;
            if (new Data[] { Data.PATH_RUN, Data.WALL }.Any(x => x == board[p.X][p.Y])) return false;
            return true;
        }

        private bool GoDown(Data[][] board, Point p)
        {
            p.IncX();       // not affect outside code because is copied to this method stack - valuetype
            if (p.X >= board.Length) return false;
            if (new Data[] { Data.PATH_RUN, Data.WALL }.Any(x => x == board[p.X][p.Y])) return false;
            return true;
        }

        private bool GoUp(Data[][] board, Point p)
        {
            p.DecX();       // not affect outside code because is copied to this method stack - valuetype
            if (p.X < 0) return false;
            if (new Data[] { Data.PATH_RUN, Data.WALL }.Any(x => x == board[p.X][p.Y])) return false;
            return true;
        }


        private String TransformValue(Data v)
        {
            switch (v)
            {
                case Data.FLOOR: return "*";
                case Data.WALL: return "#";
                case Data.PATH_RUN: return "~";
                case Data.WAY_OUT: return ">";
                default: return "?";
            }
        }

        private Data TransformValue(bool v)
        {
            return !v ? Data.FLOOR : Data.WALL;
        }


        private Data[][] MakeAcopy()
        {
            var result = new Data[m_board.Length][];

            for (int i = 0; i < m_board.Length; i++)
            {
                result[i] = new Data[m_board[0].Length];
                for (int j = 0; j < m_board[i].Length; j++)
                {
                    result[i][j] = m_board[i][j];
                }
            }
            return result;
        }

        #endregion
    }
}
