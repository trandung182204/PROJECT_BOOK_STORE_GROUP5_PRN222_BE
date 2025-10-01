using System;
using System.Collections.Generic;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

public partial class Order
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public decimal? ShippingFee { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
