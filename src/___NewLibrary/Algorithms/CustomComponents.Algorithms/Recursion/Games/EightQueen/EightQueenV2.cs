using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Recursion.Games.EightQueen
{
    public class EightQueenV2
    {
        private readonly int m_size;
        private readonly List<int[]> m_solutions;


        public EightQueenV2(int size = 8)
        {
            if (size < 4)
                throw new InvalidOperationException("board size less than 4");

            m_size = size;
            m_solutions = new List<int[]>();
        }
       
        public IEnumerable<IEnumerable<int[]>> Generate()
        {
            Func<int, IEnumerable<int[]>> executor = idx =>
            {
                // create empty board
                char[][] board = CreateEmptyBoard();

                // call code
                GenerateR(board, idx, 0);

                // here we should already have the solutions.
                return m_solutions.AsReadOnly();
            };

            for (int i = 0; i < m_size; i++)
            {
                yield return executor(i); 
            }
        }

        /// <summary>
        ///     Create a new board with not queens set.
        /// </summary>
        private char[][] CreateEmptyBoard()
        {
            char[][] board = new char[m_size][];
            for (int i = 0; i < board.Length; board[i] = new char[m_size], i++) ;
            return board;
        }


        private void GenerateR(char[][] board, int row, int col)
        {
            // if im in the last column
            if (col == board.Length - 1)
            {
                row = FindAvailablePositionFreeOfDangerous(board, row, col);

                // and it was found a line where a queen is not in dangerous.
                // -> Save current array path.

                if (row != -1)
                {
                    board[row][col] = 'Q';
                    SaveCurrentBoard(board);
                }

                return;
            }

            while ((row = FindAvailablePositionFreeOfDangerous(board, row, col)) >= 0)
            {
                // not dangerous, set up Queen
                board[row][col] = 'Q';          

                 // go recursivelly to the next column
                GenerateR(board, 0, col + 1);

                // not sucessfull queen, remove             
                board[row][col] = '\0';

                // increment row
                row++;

            }
        }

        private void SaveCurrentBoard(char[][] board)
        {
            int[] solution = new int[board.Length];

            // prepare solution
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == 'Q')
                    {
                        solution[j] = i;
                        break;
                    }
                }
            }

            bool alreadyThere = false;
            foreach (var s in m_solutions)
            {
                if (s.SequenceEqual(solution))
                {
                    alreadyThere = true;
                    break;
                }
            }

            if (!alreadyThere)
                m_solutions.Add(solution);
        }


        /// <summary>
        ///     Returns the next index where the queen can stay without running dangerous.
        ///     if there is no position available return -1.
        /// </summary>
        private static int FindAvailablePositionFreeOfDangerous(char[][] board, int row, int col)
        {
            while (row < board.Length && isDangerous(board, row, col))
                row++;

            return row == board.Length ? -1 : row;
        }



        /// <summary>
        ///     Method that check if the queen is in dangerous checking horizontaly, vertically and diagonaly.
        ///     If there are no queens in those lines, it is considered to be not in dangerous.
        /// </summary>
        private static bool isDangerous(char[][] board, int row, int col)
        {
            // check if there is any queen horizontaly.
            int i, j;
            for (j = 0; j < board.Length; j++)
            {
                if(board[row][j] == 'Q' && j != col)
                    return true;
            }

            // check if there is any queen vertically.
            for (i = 0; i < board.Length; i++)
            {
                if (board[i][col] == 'Q' && i != row)
                    return true;
            }

            // go top left until leave board
            for (i = row, j = col; --i >= 0 && --j >= 0; )
                if (board[i][j] == 'Q') return true;

            // go top right until leave board
            for (i = row, j = col; --i >= 0 && ++j < board.Length; )
                if (board[i][j] == 'Q') return true;

            // go buttom left until leave board
            for (i = row, j = col; ++i < board.Length && --j >= 0; )
                if (board[i][j] == 'Q') return true;

            // go buttom right until leave board
            for (i = row, j = col; ++i < board.Length && ++j < board.Length; )
                if (board[i][j] == 'Q') return true;

            return false;
        }

    }
}
