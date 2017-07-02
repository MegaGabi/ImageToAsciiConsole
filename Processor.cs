using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ImageToConsoleASCIIart
{
    class Processor
    {
        static char[] conv = (" .`':;*/:o&8#W@").ToCharArray();
        public coloredChar[,] res { get; private set; }
        public TimeSpan ts;

        public coloredChar[,] toASCII(Bitmap bmp_, int max_size)
        {
            DateTime dt = new DateTime();
            dt = DateTime.Now;

            Bitmap bmp = null;
            if (max_size != 0)
            {
                double ratio = (double)max_size/bmp_.Width;

                int newHeight = Convert.ToInt32(bmp_.Height * ratio);
                int newWidth = Convert.ToInt32(bmp_.Width * ratio);

                bmp = new Bitmap(bmp_, new Size(newWidth, newHeight));
            }
            else
                bmp = new Bitmap(bmp_);

            res = GetCharsSmart(bmp);

            ts = (DateTime.Now - dt);
            Console.WriteLine("Done! Time taken: " + ts.Seconds + "," + ts.Milliseconds + " sec.");
 
            return res;
        }

        private static double Brightness(Color C)
        {
            return (double)(C.R + C.G + C.B)/3;
        }

        private static coloredChar[,] GetCharsSmart(Bitmap image)
        {
            int pixhight = 4, pixwidth = 2;
            int pixseg = pixwidth * pixhight;

            int resh = image.Height / pixhight, resw = image.Width / pixwidth;
            coloredChar[,] res = new coloredChar[resh, resw];

            for (int h = 0; h < resh; h++)
            {
                int startY = (h * pixhight);

                for (int w = 0; w < resw; w++)
                {
                    int startX = (w * pixwidth);
                    int allBrightness = 0;

                    int fR = 0, fG = 0, fB = 0;
                    for (int y = 0; y < pixwidth; y++)
                    {
                        for (int x = 0; x < pixhight; x++)
                        {
                            int cY = y + startY;
                            int cX = x + startX;

                            try
                            {
                                Color c = image.GetPixel(cX, cY);

                                fR += c.R;
                                fG += c.G;
                                fB += c.B;

                                allBrightness += (c.R + c.G + c.B) / 3;
                            }
                            catch
                            {
                                allBrightness += 127;
                            }
                        }
                    }

                    fR /= pixseg;
                    fG /= pixseg;
                    fB /= pixseg;

                    Color avg_color = Color.FromArgb(fR, fG, fB);

                    int sb = (allBrightness / pixseg);

                    int tst = sb / (255 / conv.Length);
                    res[h, w].ch = tst == conv.Length ? conv.Length - 1 : tst;

                    res[h, w].rclr = avg_color;
                }
            }

            return res;
        }

        static int GetCharIndex(char ch)
        {
            return Array.IndexOf(conv, ch);
        }

        public static ConsoleColor ClosestConsoleColor(byte r, byte g, byte b)
        {
            ConsoleColor ret = 0;
            double rr = r, gg = g, bb = b, delta = double.MaxValue;

            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                var n = Enum.GetName(typeof(ConsoleColor), cc);
                var c = System.Drawing.Color.FromName(n == "DarkYellow" ? "Orange" : n); // bug fix
                var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                if (t == 0.0)
                    return cc;
                if (t < delta)
                {
                    if (!((cc == ConsoleColor.Gray || cc == ConsoleColor.DarkGray) && t > 2000))
                    {
                        delta = t;
                        ret = cc;
                    }
                }
            }

            return ret;
        }

        private static Bitmap ImagePooling(Bitmap bmp, int pool_size)
        {
            int size_plus_init = 1;
            int interval_height = 4;
            Bitmap res = new Bitmap((bmp.Width / pool_size) + 1,
                                    (int)(bmp.Height / (pool_size + (double)(size_plus_init / interval_height)) + 1));

            int size_plus = 0;
            for (int i = 0; i < bmp.Width; i += pool_size)
            {
                for (int j = 0; j < bmp.Height; j += (pool_size+ size_plus))
                {
                    size_plus = (j % interval_height == 0 ? size_plus_init : 0);
                    Color clr;
                    //Get one pixel
                    clr = Pool(bmp, i, j, pool_size + size_plus, pool_size);
                    res.SetPixel(i/pool_size, j/ (pool_size + size_plus), clr);
                }
            }

            return res;
        }

        private static Color Pool(Bitmap f, int i, int j, int py, int px)
        {
            Color maxPixel = Color.FromArgb(0, 0, 0);
            int fR = 0, fG = 0, fB = 0;

            for (int z = i - (px / 2), cz = 0; cz < py; cz++, z++)
            {
                for (int c = j - (py / 2), cc = 0; cc < px; cc++, c++)
                {
                    if ((c >= 0 && c < f.Height) && (z >= 0 && z < f.Width))
                    {
                        Color pixel = f.GetPixel(z, c);

                        //if (pixel.R > maxPixel.R && pixel.G > maxPixel.G && pixel.B > maxPixel.B)
                        //    maxPixel = pixel;

                        fR += pixel.R;
                        fG += pixel.G;
                        fB += pixel.B;
                    }
                }
            }

            int size = py * px;
            maxPixel = Color.FromArgb(fR / size, fG / size, fB / size);

            return maxPixel;
        }

        public static char NumberToChar(int idx)
        {
            return conv[idx];
        }

        public static int GetInvrtedIndex(int idx)
        {
            return (conv.Length - idx - 1);
        }

        public struct coloredChar
        {
            public int ch;
            public Color rclr;

            public coloredChar(int c, Color real)
            {
                ch = c; rclr = real;
            }
        }
    }
}
