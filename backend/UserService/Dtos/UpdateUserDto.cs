using System;
using System.ComponentModel.DataAnnotations;


public class UpdateUserDto
{
    [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
    public string? Username { get; set; }
    [StringLength(100, ErrorMessage = "Password cannot be longer than 100 characters")]
    public string? Password { get; set; }
    [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name can only contain letters")]
    public string? FirstName { get; set; }
    [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name can only contain letters")]
    public string? LastName { get; set; }

}