using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicService.Models
{
    [Table("Comments")]
    public class CommentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [MaxLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; }
        public int PicId { get; set; }
        public string UserId { get; set; }
        public DateTime CommentDate { get; set; }

        public CommentModel()
        {
            CommentDate = DateTime.UtcNow;
        }
    }
}


