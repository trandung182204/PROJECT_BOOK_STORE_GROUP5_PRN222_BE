using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public partial class BookStoreContext : DbContext
{
    public BookStoreContext()
    {
    }

    public BookStoreContext(DbContextOptions<BookStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookCategory> BookCategories { get; set; }

    public virtual DbSet<BookImage> BookImages { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<InventoryLog> InventoryLogs { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=book_store;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__activity__3213E83F78C8C94C");

            entity.ToTable("activity_logs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(255)
                .HasColumnName("action");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Detail).HasColumnName("detail");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__activity___user___681373AD");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__books__3213E83F67728AE1");

            entity.ToTable("books");

            entity.HasIndex(e => e.Code, "UQ__books__357D4CF9E0A42EA1").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Author)
                .HasMaxLength(255)
                .HasColumnName("author");
            entity.Property(e => e.AverageRating)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.AverageRating1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("average_rating");
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedAt1)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CreatedAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiscountPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("discount_price");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsDeleted1)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Language)
                .HasMaxLength(100)
                .HasColumnName("language");
            entity.Property(e => e.PageCount).HasColumnName("page_count");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("price");
            entity.Property(e => e.PrintType)
                .HasMaxLength(100)
                .HasColumnName("print_type");
            entity.Property(e => e.Publisher)
                .HasMaxLength(255)
                .HasColumnName("publisher");
            entity.Property(e => e.SoldCount).HasDefaultValue(0);
            entity.Property(e => e.SoldCount1)
                .HasDefaultValue(0)
                .HasColumnName("sold_count");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.StockQuantity)
                .HasDefaultValue(0)
                .HasColumnName("stock_quantity");
            entity.Property(e => e.ThumbnailUrl)
                .HasMaxLength(500)
                .HasColumnName("thumbnail_url");
            entity.Property(e => e.ThumbnailUrl1)
                .HasMaxLength(500)
                .HasColumnName("ThumbnailUrl");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedAt1).HasColumnName("UpdatedAt");
        });

        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__book_cat__3213E83F8E00D2F5");

            entity.ToTable("book_categories");

            entity.HasIndex(e => e.BookId, "IX_book_categories_book_id");

            entity.HasIndex(e => e.CategoryId, "IX_book_categories_category_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");

            entity.HasOne(d => d.Book).WithMany(p => p.BookCategories)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__book_cate__book___45BE5BA9");

            entity.HasOne(d => d.Category).WithMany(p => p.BookCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__book_cate__categ__46B27FE2");
        });

        modelBuilder.Entity<BookImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__book_ima__3213E83F1133BAA0");

            entity.ToTable("book_images");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");

            entity.HasOne(d => d.Book).WithMany(p => p.BookImages)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__book_imag__book___5E8A0973");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__carts__3213E83FA9386628");

            entity.ToTable("carts");

            entity.HasIndex(e => e.UserId, "IX_carts_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__carts__user_id__1CBC4616");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cart_ite__3213E83F126BE46B");

            entity.ToTable("cart_items");

            entity.HasIndex(e => e.BookId, "IX_cart_items_book_id");

            entity.HasIndex(e => e.CartId, "IX_cart_items_cart_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");

            entity.HasOne(d => d.Book).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__cart_item__book___2180FB33");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__cart_item__cart___208CD6FA");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83F3D6EB9C1");

            entity.ToTable("categories");

            entity.HasIndex(e => e.CategoryCode, "UQ__categori__BC9D1E7CDD7A9C13").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryCode)
                .HasMaxLength(100)
                .HasColumnName("category_code");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
        });

        modelBuilder.Entity<InventoryLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__inventor__3213E83FECEE7682");

            entity.ToTable("inventory_logs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.ChangeType)
                .HasMaxLength(20)
                .HasColumnName("change_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.QuantityChange).HasColumnName("quantity_change");

            entity.HasOne(d => d.Book).WithMany(p => p.InventoryLogs)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__inventory__book___634EBE90");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__orders__3213E83F738531E8");

            entity.ToTable("orders");

            entity.HasIndex(e => e.UserId, "IX_orders_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(20)
                .HasDefaultValue("PENDING")
                .HasColumnName("order_status");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .HasDefaultValue("COD")
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("UNPAID")
                .HasColumnName("payment_status");
            entity.Property(e => e.ShippingAddress).HasColumnName("shipping_address");
            entity.Property(e => e.ShippingFee)
                .HasDefaultValue(0.0m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("shipping_fee");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__orders__user_id__2BFE89A6");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__order_it__3213E83F5C6D1A1D");

            entity.ToTable("order_items");

            entity.HasIndex(e => e.BookId, "IX_order_items_book_id");

            entity.HasIndex(e => e.OrderId, "IX_order_items_order_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Book).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__order_ite__book___30C33EC3");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__order_ite__order__2FCF1A8A");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payments__3213E83FF91DD0E9");

            entity.ToTable("payments");

            entity.HasIndex(e => e.OrderId, "IX_payments_order_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaidAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("paid_at");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .HasColumnName("payment_method");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .HasColumnName("transaction_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__payments__order___4F47C5E3");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__reviews__3213E83FC6757D82");

            entity.ToTable("reviews");

            entity.HasIndex(e => e.BookId, "IX_reviews_book_id");

            entity.HasIndex(e => e.UserId, "IX_reviews_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsApproved)
                .HasDefaultValue(true)
                .HasColumnName("is_approved");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Book).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__reviews__book_id__540C7B00");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__reviews__user_id__55009F39");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F83CF44CE");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164C519C7E3").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__users__F3DBC57266717BA7").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.AvatarUrl1)
                .HasMaxLength(500)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedAt1)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CreatedAt");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(255)
                .HasColumnName("fullname");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsDeleted1)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("CUSTOMER")
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedAt1).HasColumnName("UpdatedAt");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__vouchers__3213E83F7395D289");

            entity.ToTable("vouchers");

            entity.HasIndex(e => e.Code, "UQ__vouchers__357D4CF9836694F0").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(10)
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("discount_value");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.MinOrderValue)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("min_order_value");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.UsageLimit)
                .HasDefaultValue(1)
                .HasColumnName("usage_limit");
            entity.Property(e => e.UsedCount)
                .HasDefaultValue(0)
                .HasColumnName("used_count");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
