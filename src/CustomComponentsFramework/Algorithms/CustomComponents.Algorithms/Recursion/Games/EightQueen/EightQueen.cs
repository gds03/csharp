using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Recursion.Games.EightQueen
{
    public class EightQueen
    {
        public int ChessboardSize { get; private set; }
        public int[][] Board { get; private set; }


        public EightQueen(int chessboard_size)
        {
            if (chessboard_size <= 3)
                throw new ArgumentException("chessboard_size <= 3 ");

            ChessboardSize = chessboard_size;
        }


        public int[][] Generate()
        {
            int combinations_count = ChessboardSize * ChessboardSize;
            int combinations = 0;

            // create memory matrix
            int[][] data = null;
            int insertedQueens;
            
            int line = 0; int column = -1; int column_reseted_times = 0;
            do
            {                
                if (column + 1 == ChessboardSize) {
                    column = 0;
                    column_reseted_times++;

                    if (column_reseted_times == ChessboardSize)
                    {
                        column_reseted_times = 0;
                        line++;
                    }
                }
                else column++;

                // create matrix and try to fill starting the position from line and column [with previous state]
                data = newArray();
                insertedQueens = 0;
                for (int x = line; x < data.Length; x++)
                {
                    for (int y = column; y < data[x].Length; y++)
                    {
                        if (!isDangerous(data, x, y))
                        {
                            data[x][y] = 1;
                            insertedQueens++;

                            if (insertedQueens == ChessboardSize) { 
                                Board = data; return Board; 
                            }
                                
                        }
                    }
                }

                combinations++;
            }
            while (combinations < combinations_count); 
             
            return null;
        }

        private int[][] newArray()
        {
            int[][]  data = new int[ChessboardSize][];
            for (int k = 0; k < data.Length; data[k] = new int[ChessboardSize], k++) ;
            return data;
        }

       


        private bool isDangerous(int[][] data, int row, int col)
        {
            // check if there is any queen horizontaly.
            int i, j;
            for (j = 0; j < ChessboardSize; j++)
            {
                if(data[row][j] == 1 && j != col)
                    return true;
            }

            // check if there is any queen vertically.
            for ( i = 0; i < ChessboardSize; i++)
            {
                if (data[i][col] == 1 && i != row)
                    return true;
            }

            // go top left until leave board
            for (i = row, j = col; --i >= 0 && --j >= 0; )
                if (data[i][j] == 1) return true;

            // go top right until leave board
            for (i = row, j = col; --i >= 0 && ++j < ChessboardSize; )
                if (data[i][j] == 1) return true;

            // go buttom left until leave board
            for (i = row, j = col; ++i < ChessboardSize && --j >= 0; )
                if (data[i][j] == 1) return true;

            // go buttom right until leave board
            for (i = row, j = col; ++i < ChessboardSize && ++j < ChessboardSize; )
                if (data[i][j] == 1) return true;

            return false;
        }

    }
}
