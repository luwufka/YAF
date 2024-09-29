using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

namespace YAF.YoTools
{
    internal class Program
    {
        static Dictionary<string, string> helpBase = new Dictionary<string, string>()
        {
            { "--help", "Get all the commands and arguments to them." },
            { "--pack", "A pack of frames in YAF. Arguments: path-to-frames ; output.yaf" },
            { "--extract", "Decompress frames from YAF to a directory. Arguments: input.yaf ; path-to-output-dir" },
            { "--get_info", "Retrieves information about the YAF file: size, weight, and frame information. Argument: input.yaf" }
        };

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string main = args[0];
                if (main == "--help")
                {
                    Help();
                }
                if (main == "--pack")
                {
                    File.WriteAllBytes(args[2], YoLib.YAF.Pack(Utils.LoadImages(args[1])));
                }
                if (main == "--extract")
                {
                    int i = 0;
                    List<Bitmap> frames = YoLib.YAF.Unpack(File.ReadAllBytes(args[1]));

                    Directory.CreateDirectory(args[2]);

                    foreach (Bitmap frame in frames)
                    {
                        frame.Save(Path.Combine(args[2], $"{i}.png"), System.Drawing.Imaging.ImageFormat.Png);
                        i++;
                    }
                }
                if (main == "--get_info")
                {
                    string yafPath = args[1];
                    FileInfo yafFileInfo = new FileInfo(yafPath);
                    int i = 0;
                    List<Bitmap> frames = YoLib.YAF.Unpack(File.ReadAllBytes(yafPath));

                    Console.WriteLine($"Size: {yafFileInfo.Length / 1024.0} KB.\n\n[ Frames: ]");
                    foreach (Bitmap frame in frames)
                    {
                        Console.WriteLine($"Frame #{i} - {frame.Width} x {frame.Height}");
                        i++;
                    }
                }
            }
            else
            {
                Help();
                Thread.Sleep(3000);
            }
        }

        static void Help()
        {
            Console.WriteLine("[ YoTools Commands: ]");
            foreach (KeyValuePair<string, string> part in helpBase)
            {
                Console.WriteLine($"{part.Key} - {part.Value}");
            }
        }
    }
}
