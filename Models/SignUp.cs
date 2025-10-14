using System.ComponentModel.DataAnnotations;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public class SignUp
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string? ConfirmPassword { get; set; }
    [Required]
    public string  FullName { get; set; } = null!;
    [Required]
    public string Address { get; set; } = null!;
    [Required]
    public string AvatarUrl { get; set; } = null!;

}