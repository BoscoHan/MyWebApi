using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Models
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Description { get; set; }

        public double Price { get; set; }

        public int quantity { get; set; }

        public byte[] ImgByte { get; set; }
    }
}
