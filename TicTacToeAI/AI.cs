using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeAI
{
    public class AI
    {
        protected int[,] Field { get; private set; }
        public AI(int[,] field)
        {
            Field = field;
        }

        static AI()
        {
            Console.WriteLine("AI was been created");
        }

        public override string ToString()
        {
            return "Information:\n Version: v0.0.1";
        }
    }
}
