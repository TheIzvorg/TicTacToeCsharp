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
    }
}
