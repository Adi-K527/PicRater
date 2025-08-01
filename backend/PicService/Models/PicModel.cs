using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicService.Models
{
    [Table("Pics")]
    public class PicModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PicId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [MinLength(3, ErrorMessage = "Title must be at least 3 characters long")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Title can only contain alphanumeric characters and spaces")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
        public string UserId { get; set; }
        public DateTime UploadDate { get; set; }

        public PicModel()
        {
            UploadDate = DateTime.UtcNow;
        }
    }
}