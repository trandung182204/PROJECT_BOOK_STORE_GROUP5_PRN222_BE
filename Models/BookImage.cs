using System;
using System.Collections.Generic;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public partial class BookImage
{
    public long Id { get; set; }

    public long BookId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Book Book { get; set; } = null!;
}
