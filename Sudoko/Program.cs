using Newtonsoft.Json;
using System;
using System.IO;

namespace Sudoko
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            var input = new int[9, 9];
            input = JsonConvert.DeserializeObject<int[,]>(File.ReadAllText("medium.txt"));
            var s = new Sudoku(input);
            s.PrintToConsole();
            s.Solve();
            s.PrintToConsole();
            Console.ReadLine();
        }
    }
}
