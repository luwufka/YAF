using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace YAF.YoLib
{
    public static class YAF
    {
        public static List<Bitmap> Unpack(byte[] inputData)
        {
            List<Bitmap> frames = new List<Bitmap>();

            using (BinaryReader reader = new BinaryReader(new MemoryStream(inputData)))
            {
                Utils.ValidateFormat(reader);
                int frameCount = reader.ReadInt32();

                for (int i = 0; i < frameCount; i++)
                {
                    frames.Add(Utils.ReadFrame(reader));
                }
            }

            return frames;
        }

        public static byte[] Pack(List<Bitmap> frames)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    Utils.WriteHeader(writer, frames.Count);

                    foreach (var frame in frames)
                    {
                        Utils.WriteFrame(writer, frame);
                    }
                }
                return memoryStream.ToArray();
            }
        }
    }
}
