using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Intrinsics.X86;

namespace MyWebApi.Services
{
    public class ImageService
    {
        public byte[] ReadAllBytes(string fileName)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
            }

            int MB = 1024 * 1024;
            //probably don't want to save something too large to dB as it's expensive
            if (buffer.Length > 100*MB)
            {
                throw new ArgumentException("File being processed exceeds 100MB, which exceeds the upload limit");
            }
            return buffer;
        }
    }
}
