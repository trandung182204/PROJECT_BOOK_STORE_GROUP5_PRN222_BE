using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Data;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;
[Table("RefreshToken")]
public class RefreshToken
{
     [Key]
     public Guid Id { get; set; }
     
     public string? UserId { get; set; }
     [ForeignKey("UserId")]     
     public ApplicationUser User { get; set; } = null!;
     
     public string? Token { get; set; }
     
     public string? JwtId { get; set; }
     
     public bool IsUsed { get; set; }
     
     public bool IsRevoked { get; set; }
     
     public DateTime IssuedAt { get; set; }
     
     public DateTime ExpiredAt { get; set; }
}