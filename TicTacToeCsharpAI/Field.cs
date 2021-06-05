using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeCsharp
{
    
    class Field
    {
        
        const int dimensionSize = 3;
        private int[,] field = new int[dimensionSize, dimensionSize];
        public int[,] getField()
        {
            return field;
        }
        public Field()
        {
            for(int i = 0; i < field.Length; i++)
                field[i / dimensionSize, i % dimensionSize] = 0;
        }
        enum STATUS
        {
            ZERO = 0,
            X = 1,
            O = 2
        }
        public void ShowField()
        {
            /*for(int i = 0; i < field.Length; i++)
            {
                if (i % 3 == 0) 
                    Console.WriteLine();
                if (field[i/dimensionSize,i%dimensionSize] == (int)STATUS.ZERO)
                    Console.Write("  ");
                else if (field[i / dimensionSize, i % dimensionSize] == (int)STATUS.X)
                    Console.Write(" X");
                else if (field[i / dimensionSize, i % dimensionSize] == (int)STATUS.O)
                    Console.Write(" O");
            }*/
            
            for (int i = 0; i < dimensionSize; i++)
            {
                for (int j = 0; j < dimensionSize; j++)
                {
                    if (field[i, j] == 0)
                    {
                        Console.Write("  ");
                    }
                    else if (field[i, j] == 1)
                    {
                        Console.Write(" X");
                    }
                    else if (field[i, j] == 2)
                    {
                        Console.Write(" O");
                    }
                    else
                    {
                        throw new Exception("Не может быть такого значения");
                    }
                    //Console.Write(field[i,j]);
                }
                Console.WriteLine();
            }
        }
    }
}
