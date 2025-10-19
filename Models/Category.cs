using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public partial class Category
{
    public long Id { get; set; }

    public string CategoryCode { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public long? ParentId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    [JsonIgnore]
    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();
    [JsonIgnore]
    public virtual Category? Parent { get; set; }
}
