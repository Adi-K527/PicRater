using System;
using System.ComponentModel.DataAnnotations;

public class CommentDto
{
    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; }
}