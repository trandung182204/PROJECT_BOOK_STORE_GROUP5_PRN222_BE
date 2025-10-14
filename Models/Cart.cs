using System;
using System.Collections.Generic;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Data;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public partial class Cart
{
    public long Id { get; set; }

    public string? UserId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ApplicationUser User { get; set; }
}
