using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToConsoleASCIIart
{
    class ProgressBar
    {
        public int size { get; private set; } = 0;
        int current = 0;
        int real_size = 0;
        public int y { get; private set; } = 0;
        int x = 0;
        char pb_Char = '█';
        double onePercent = 0;
        public ProgressBar(int rsize, int psize = 10)
        {
            real_size = rsize;
            size = psize;
            current = 0;
            onePercent = (rsize / 100.0);
        }

        public void start()
        {
            y = Console.CursorTop;
            Console.Write("[" + new string(' ', size) + "]");

            Console.SetCursorPosition(1,y);
            Console.Write(pb_Char);
        }

        public void Progress(int cur)
        {
            int ps =  GetCurrentPos(cur);
            while (ps > current)
            {
                Console.SetCursorPosition(current + 2, y);
                Console.Write("█");//
                current++;
            }
        }

        public void End()
        {
            while (current < (size - 1))
            {
                Console.SetCursorPosition(current + 2, y);
                Console.Write("█");//
                current++;
            }
            Console.WriteLine();
        }

        private int GetCurrentPos(int cur)
        {
            return (cur * 100) / real_size / 5;
        }
    }
}
