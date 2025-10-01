using System;
using System.Collections.Generic;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public partial class BookCategory
{
    public long Id { get; set; }

    public long BookId { get; set; }

    public long CategoryId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;
}
