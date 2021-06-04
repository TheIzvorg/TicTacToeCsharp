using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToeAI;

namespace TicTacToeCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Field field = new Field();
            AI bot = new AI(field.getField());
            Console.ReadLine();
        }
    }
}
