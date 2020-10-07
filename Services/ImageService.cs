using System;
using System.IO;

namespace MyWebApi.Services
{
    public class ImageService
    {
        static readonly int MB = 1024 * 1024;
        static readonly long maxSizeAllowed = 100 * MB;

        public byte[] ReadAllBytes(string fileName)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
            }

            //probably don't want to save something too large to dB as it's expensive
            if (buffer.Length > maxSizeAllowed)
            {
                throw new ArgumentException("File being processed exceeds 100MB, which exceeds the upload limit");
            }
            return buffer;
        }
    }
}
