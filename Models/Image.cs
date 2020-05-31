using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApi.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Description { get; set; }

        public double Price { get; set; }

        public int quantity { get; set; }

        public byte[] ImgByte { get; set; }
    }
}
