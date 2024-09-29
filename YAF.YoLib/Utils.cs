using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace YAF.YoLib
{
    public static class Utils
    {
        public static void ValidateFormat(BinaryReader reader)
        {
            if (new string(reader.ReadChars(3)) != "YAF")
                throw new Exception("Invalid format! This is not a YAF.");
        }

        public static Bitmap ReadFrame(BinaryReader reader)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            Bitmap frame = new Bitmap(width, height);
            byte[] compressedData = reader.ReadBytes(reader.ReadInt32());

            using (var compressedStream = new MemoryStream(compressedData))
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int argb = new BinaryReader(gzipStream).ReadInt32();
                        frame.SetPixel(x, y, Color.FromArgb(argb));
                    }
                }
            }

            return frame;
        }

        public static void WriteHeader(BinaryWriter writer, int frameCount)
        {
            writer.Write(new char[] { 'Y', 'A', 'F' });
            writer.Write(frameCount);
        }

        public static void WriteFrame(BinaryWriter writer, Bitmap frame)
        {
            writer.Write(frame.Width);
            writer.Write(frame.Height);

            byte[] compressedData = CompressFrame(frame);
            writer.Write(compressedData.Length);
            writer.Write(compressedData);
        }

        public static byte[] CompressFrame(Bitmap frame)
        {
            using (var compressedStream = new MemoryStream())
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                for (int y = 0; y < frame.Height; y++)
                {
                    for (int x = 0; x < frame.Width; x++)
                    {
                        int argb = frame.GetPixel(x, y).ToArgb();
                        byte[] pixelData = BitConverter.GetBytes(argb);
                        gzipStream.Write(pixelData, 0, pixelData.Length);
                    }
                }

                gzipStream.Flush();
                return compressedStream.ToArray();
            }
        }


    }
}
