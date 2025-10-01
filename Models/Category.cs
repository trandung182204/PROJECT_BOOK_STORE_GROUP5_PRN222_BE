using System;
using System.Collections.Generic;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public partial class Category
{
    public long Id { get; set; }

    public string CategoryCode { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}
