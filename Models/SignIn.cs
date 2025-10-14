using System.ComponentModel.DataAnnotations;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public class SignIn
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}