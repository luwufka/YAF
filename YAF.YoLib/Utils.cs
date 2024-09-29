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

        public static void WriteHeader(BinaryWriter writer, int frameCount)
        {
            writer.Write(new char[] { 'Y', 'A', 'F' });
            writer.Write(frameCount);
        }

        public static void WriteFrame(BinaryWriter writer, Bitmap frame)
        {
            writer.Write(frame.Width);
            writer.Write(frame.Height);

            using (MemoryStream pixelStream = new MemoryStream())
            {
                using (BinaryWriter pixelWriter = new BinaryWriter(pixelStream))
                {
                    for (int y = 0; y < frame.Height; y++)
                    {
                        for (int x = 0; x < frame.Width; x++)
                        {
                            Color pixel = frame.GetPixel(x, y);
                            pixelWriter.Write(pixel.ToArgb());
                        }
                    }
                }

                byte[] pixelData = pixelStream.ToArray();

                using (MemoryStream compressedStream = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                    {
                        gzipStream.Write(pixelData, 0, pixelData.Length);
                    }

                    byte[] compressedData = compressedStream.ToArray();
                    writer.Write(compressedData.Length);
                    writer.Write(compressedData);
                }
            }
        }

        public static byte[] CompressFrame(Bitmap frame)
        {
            using (MemoryStream pixelStream = new MemoryStream())
            {
                for (int y = 0; y < frame.Height; y++)
                {
                    for (int x = 0; x < frame.Width; x++)
                    {
                        int argb = frame.GetPixel(x, y).ToArgb();
                        pixelStream.Write(BitConverter.GetBytes(argb), 0, 4);
                    }
                }

                using (MemoryStream compressedStream = new MemoryStream())
                using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                {
                    pixelStream.Position = 0;
                    pixelStream.CopyTo(gzipStream);
                    gzipStream.Flush();
                    return compressedStream.ToArray();
                }
            }
        }

        public static Bitmap ReadFrame(BinaryReader reader)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            int compressedDataLength = reader.ReadInt32();
            byte[] compressedData = reader.ReadBytes(compressedDataLength);

            using (MemoryStream compressedStream = new MemoryStream(compressedData))
            using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                Bitmap frame = new Bitmap(width, height);
                byte[] pixelData = new byte[4 * width];

                for (int y = 0; y < height; y++)
                {
                    int bytesRead = gzipStream.Read(pixelData, 0, pixelData.Length);

                    for (int x = 0; x < width; x++)
                    {
                        Color color = Color.FromArgb(BitConverter.ToInt32(pixelData, x * 4));
                        frame.SetPixel(x, y, color);
                    }
                }

                return frame;
            }
        }
    }
}