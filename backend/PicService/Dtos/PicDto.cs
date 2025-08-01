using System;
using System.ComponentModel.DataAnnotations;

public class PicDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    public string Description { get; set; }

    public string UserId { get; set; }

}