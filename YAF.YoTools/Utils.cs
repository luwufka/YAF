using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace YAF.YoTools
{
    public static class Utils
    {
        public static List<Bitmap> LoadImages(string path)
        {
            List<Bitmap> frames = new List<Bitmap>();
            var files = Directory.GetFiles(path, "*.png")
            .OrderBy(f => int.TryParse(Path.GetFileNameWithoutExtension(f), out var num) ? num : int.MaxValue)
            .ToList();

            foreach (var file in files)
            {
                frames.Add(new Bitmap(file));
            }

            return frames;
        }
    }
}
