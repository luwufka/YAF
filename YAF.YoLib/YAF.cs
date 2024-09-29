using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace YAF.YoLib
{
    public static class YAF
    {
        public static List<Bitmap> Unpack(byte[] input)
        {
            using (var memoryStream = new MemoryStream(input))
            using (var reader = new BinaryReader(memoryStream))
            {
                Utils.ValidateFormat(reader);

                var frames = new List<Bitmap>();
                int frameCount = reader.ReadInt32();

                for (int i = 0; i < frameCount; i++)
                {
                    Bitmap frame = Utils.ReadFrame(reader);
                    frames.Add(frame);
                }

                return frames;
            }
        }

        public static byte[] Pack(List<Bitmap> frames)
        {
            using (var outputStream = new MemoryStream())
            using (var writer = new BinaryWriter(outputStream))
            {
                Utils.WriteHeader(writer, frames.Count);

                foreach (Bitmap frame in frames)
                {
                    Utils.WriteFrame(writer, frame);
                }

                return outputStream.ToArray();
            }
        }
    }
}
