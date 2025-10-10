using System;
using System.Collections.Generic;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public partial class InventoryLog
{
    public long Id { get; set; }

    public long BookId { get; set; }

    public string ChangeType { get; set; } = null!;

    public int QuantityChange { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Book Book { get; set; } = null!;
}
