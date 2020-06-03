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

        public int Quantity { get; set; }

        public bool Isprivate { get; set; }

        //FK for user:
        //Attached file must have user attached, easier to sort out permissions
        public int UserId { get; set; }
        public User User { get; set; }
        public byte[] ImgByte { get; set; }
    }

    public class InsertImageModel : Image
    {
        public string Path { get; set; }
    }

    public class UpdateImageUserModel : InsertImageModel
    {
        public int CurrentUserId { get; set; }
    }
}
