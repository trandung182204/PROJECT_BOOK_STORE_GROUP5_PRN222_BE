using System;
using System.Collections.Generic;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public partial class Review
{
    public long Id { get; set; }

    public long BookId { get; set; }

    public long UserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
