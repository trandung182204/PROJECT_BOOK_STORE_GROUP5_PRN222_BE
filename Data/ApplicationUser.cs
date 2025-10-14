using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Data;

public class ApplicationUser : IdentityUser
{
    // Các field mở rộng theo schema users hiện có
    public string  FullName { get; set; }
    public string Address { get; set; }
    public string AvatarUrl { get; set; }
    public bool IsDeleted { get; set; } = false; 
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // navigation example
    public ICollection<Order> Orders { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<ActivityLog> ActivityLogs { get; set; }
    public ICollection<Cart> Carts { get; set; }
}