using System;
using System.Collections.Generic;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Data;
namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public partial class ActivityLog
{
    public long Id { get; set; }

    public string? UserId { get; set; }

    public string? Action { get; set; }

    public string? Detail { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ApplicationUser User { get; set; }
}
