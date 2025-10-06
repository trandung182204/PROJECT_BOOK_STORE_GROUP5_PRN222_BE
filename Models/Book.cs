using System;
using System.Collections.Generic;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public partial class Book
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Author { get; set; }

    public string? Publisher { get; set; }

    public int? PageCount { get; set; }

    public string? PrintType { get; set; }

    public string? Language { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? DiscountPrice { get; set; }

    public int? StockQuantity { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? ImageUrl { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public decimal? AverageRating { get; set; }

    public int? SoldCount { get; set; }

    public string? ThumbnailUrl1 { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt1 { get; set; }

    public DateTime? UpdatedAt1 { get; set; }

    public decimal? AverageRating1 { get; set; }

    public int? SoldCount1 { get; set; }

    public bool? IsDeleted1 { get; set; }

    public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

    public virtual ICollection<BookImage> BookImages { get; set; } = new List<BookImage>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
