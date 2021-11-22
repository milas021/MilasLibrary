using System;
using System.Collections.Generic;
using System.Text;

namespace MilasLibrary
{
    public class ProgressBar
    {
        public ProgressBar()
        {
            Console.SetCursorPosition(0, 2);
        }
        public  int Percent { get; private set; }
        public  void Config(decimal i, decimal total)
        {
            Percent = Convert.ToInt32((i / total) * 100);
            Display(Percent);


        }
        private  void Display(int percent)
        {
            var top = Console.CursorTop;
            int count = percent / 1;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append("#");
            }

            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"{percent} % ");

            Console.SetCursorPosition(0, 1);
            Console.Write("#");
            Console.SetCursorPosition(101, 1);
            Console.Write("#");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(1, 1);
            Console.WriteLine(sb.ToString());
            Console.ForegroundColor = ConsoleColor.White;

            Console.CursorTop = top;


        }
    }
}
