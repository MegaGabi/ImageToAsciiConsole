using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ImageToConsoleASCIIart.Processor;

namespace ImageToConsoleASCIIart
{
    class Saver
    {
        public TimeSpan ts;

        public Bitmap Save(coloredChar[,] toShow, bool colored, bool real_color = true)
        {
            DateTime dt = new DateTime();
            dt = DateTime.Now;

            string toSave = "";
            string one = new string(' ', toShow.GetLength(1)) + "\n";
            int h = toShow.GetLength(0);

            toSave += new StringBuilder(one.Length * h).Insert(0, one, h).ToString();

            Size textsize = new Size(h, toSave.Length/h);

            int font_size = 10;
            if (textsize.Width > 60)
                font_size = 10;
            else if (textsize.Width > 40)
                font_size = 14;
            else if (textsize.Width > 20)
                font_size = 18;
            else
                font_size = 22;

            Size size = TextRenderer.MeasureText(toSave, new Font("Consolas", font_size, FontStyle.Regular, GraphicsUnit.Point));
            //Console.WriteLine("Full size: " + size.ToString());
            Bitmap res = new Bitmap(size.Width, size.Height);

            Graphics g = Graphics.FromImage(res);

            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            g.FillRectangle(Brushes.Black,0,0,res.Width,res.Height);//BLACK

            Size s = TextRenderer.MeasureText("#", new Font("Consolas", font_size, FontStyle.Regular, GraphicsUnit.Point));

            for (int i = 0; i < toShow.GetLength(0); i++)
            {
                for (int j = 0; j < toShow.GetLength(1); j++)
                {
                    int chridx;
                    Brush toColor = Brushes.White;

                    chridx = toShow[i, j].ch;

                    if (colored && real_color)
                        toColor = new SolidBrush(toShow[i, j].rclr);
                    else if (colored && !real_color)
                    {
                        Color c = toShow[i, j].rclr;
                        ConsoleColor cclr = ClosestConsoleColor(c.R, c.G, c.B);

                        toColor = new SolidBrush(FromConsole(cclr));
                    }

                    string chr = NumberToChar(chridx).ToString();

                    g.DrawString(chr, new Font("Consolas", font_size), toColor, j * s.Width/2, i * s.Height);//allert
                }
                g.DrawString("\n", new Font("Consolas", font_size), Brushes.White, 0, i * s.Height);
            }

            g.Flush();

            ts = (DateTime.Now - dt);
            Console.WriteLine("Saved! Time taken: " + ts.Seconds + "," + ts.Milliseconds + " sec.");

            return res;
        }

        Color FromConsole(ConsoleColor cc)
        {
            Color get;
            if (cc == ConsoleColor.DarkYellow)
                get = Color.Yellow;
            else
                get = Color.FromName(cc.ToString());
            
            return get;
        }
    }
}
