using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImageToConsoleASCIIart.Processor;

namespace ImageToConsoleASCIIart
{
    class ConsoleShow
    {
        public void ShowASCII(coloredChar[,] toShow, bool colored)
        {
            if (toShow.GetLength(1) > Console.WindowWidth)
            {
                Exception e = new Exception("Too big!");
                throw e;          
            }

            if (colored)
            {
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
            }

            for (int i = 0; i < toShow.GetLength(0); i++)
            {
                for (int j = 0; j < toShow.GetLength(1); j++)
                {
                    int chridx;

                    chridx = toShow[i, j].ch;
                    if (colored)
                    {
                        Color c = toShow[i, j].rclr;
                        Console.ForegroundColor = ClosestConsoleColor(c.R, c.G, c.B);
                    }
                    else
                        chridx = GetInvrtedIndex(toShow[i, j].ch);


                    Console.Write(NumberToChar(chridx));
                }
                Console.Write("\n");
            }
        }

        public void ErrorMessage(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
        }
    }
}
