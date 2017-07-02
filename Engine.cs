using Gif.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImageToConsoleASCIIart.Input;
using static ImageToConsoleASCIIart.Processor;

namespace ImageToConsoleASCIIart
{
    class Engine
    {
        bool gif = false;
        coloredChar[][,] GIF_Frames;
        int gif_speed = 10;
        int MaxSize = 0;

        public void Start()
        {
            Input inp = new Input();
            Processor prcsr = new Processor();
            ConsoleShow cs = new ConsoleShow();
            Saver sr = new Saver();

            do
            {
                Console.Write("Enter command: "); inp.ProcessInput((Console.ReadLine()).ToLower());
                Console.WriteLine(inp.LastResult.ToString());

                if (inp.LastResult == command_type.load)
                {
                    try
                    {
                        if (inp.FileName.EndsWith(".gif"))
                        {
                            gif = true;

                            Image img = Image.FromFile(inp.FileName);
                            int size = img.GetFrameCount(FrameDimension.Time);
                            PropertyItem item = img.GetPropertyItem(0x5100);
                            gif_speed = (item.Value[0] + item.Value[1] * 256) * 10;

                            Console.WriteLine(size + " frames to animate");

                            GIF_Frames = new coloredChar[size][,];

                            ProgressBar pb = new ProgressBar(size, 20);

                            pb.start();

                            DateTime dt = new DateTime();
                            dt = DateTime.Now;
                            for (int i = 0; i < size; i++)
                            {
                                //Console.WriteLine((i + 1) + "/" + size + " frame");
                                img.SelectActiveFrame(FrameDimension.Time, i);

                                pb.Progress(i);

                                Console.SetCursorPosition(pb.size + 2, pb.y);

                                Bitmap b = new Bitmap(img);

                                GIF_Frames[i] = prcsr.toASCII(b, MaxSize);

                                b.Dispose();
                            }

                            pb.End();
                            Console.WriteLine("Total time taken: " + (DateTime.Now - dt));
                        }
                        else
                        {
                            gif = false;

                            Bitmap b = new Bitmap(inp.FileName);
                            cs.ShowASCII(prcsr.toASCII(b, MaxSize), false);
                            cs.ShowASCII(prcsr.res, true);
                            b.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        cs.ErrorMessage(ex.Message);
                    }
                }
                else if (inp.LastResult == command_type.save)
                {
                    SaveImg(prcsr, sr, true);
                    SaveImg(prcsr, sr, false);
                }
                else if (inp.LastResult == command_type.saveb)
                {
                    SaveImg(prcsr, sr, false);
                }
                else if (inp.LastResult == command_type.savec)
                {
                    SaveImg(prcsr, sr, true);
                }
                else if (inp.LastResult == command_type.size)
                {
                    MaxSize = inp.MaxSize;
                }
                else if (inp.LastResult == command_type.exit)
                {
                    Environment.Exit(0);
                }

                while (Console.KeyAvailable)
                {
                    Console.ReadKey(false);
                }
                Console.Write("Нажмите любую кнопку, чтобы продолжить"); Console.ReadKey(false);

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Clear();
            } while (inp.LastResult != command_type.exit);
        }

        private void SaveImg(Processor prcsr, Saver sr, bool colored)
        {
            if (prcsr.res != null)
            {
                if (gif)
                {
                    //Console.WriteLine("Animating...");

                    AnimatedGifEncoder e = new AnimatedGifEncoder();

                    e.SetQuality(20);

                    string fname = "out_anim" + (colored ? "_colored" : "_bw") + ".gif";

                    if (File.Exists(fname))
                        File.Delete(fname);

                    e.Start(fname);
                    e.SetDelay(gif_speed);
                    e.SetRepeat(0);
                    int s = GIF_Frames.GetLength(0);

                    ProgressBar pb = new ProgressBar(s, 20);

                    pb.start();

                    DateTime dt = new DateTime();
                    dt = DateTime.Now;
                    for (int i = 0; i < s; i++)
                    {
                        pb.Progress(i);

                        Console.SetCursorPosition(pb.size + 2, pb.y);

                        Bitmap b = new Bitmap(sr.Save(GIF_Frames[i], colored));

                        b = new Bitmap(b, b.Width / 2, b.Height / 2);
                        e.AddFrame(b);
                        b.Dispose();
                    }

                    pb.End();
                    Console.WriteLine("Total time taken: " + (DateTime.Now - dt));

                    e.Finish();

                    e.SetDispose(-1);
                }
                else
                {
                    sr.Save(prcsr.res, colored).Save("out_ASCII" + (colored?"_colored":"_bw") + ".png");
                }
            }
        }
    }
}
