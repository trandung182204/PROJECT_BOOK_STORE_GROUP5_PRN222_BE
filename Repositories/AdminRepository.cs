using Microsoft.EntityFrameworkCore;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Models;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly BookStoreContext _db;
        public AdminRepository(BookStoreContext db) => _db = db;

        public async Task<object> GetSummaryAsync()
        {
            var users = await _db.Users.CountAsync(u => !u.IsDeleted);
            var orders = await _db.Orders.CountAsync();
            var revenue = await _db.Orders
                .Where(o => o.PaymentStatus == "PAID")
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
            var booksActive = await _db.Books.CountAsync(b => (b.IsDeleted ?? false) == false);
            var totalSold = await _db.OrderItems
                .Where(oi => _db.Orders
                    .Where(o => o.PaymentStatus == "PAID")
                    .Select(o => o.Id)
                    .Contains(oi.OrderId))
                .SumAsync(oi => (int?)oi.Quantity) ?? 0;

            return new { users, orders, revenue, booksActive, totalSold };
        }

        public async Task<IEnumerable<object>> GetSalesReportAsync(string type, DateTime? from, DateTime? to)
        {
            var q = _db.Orders.AsNoTracking().Where(o => o.PaymentStatus == "PAID");
            if (from.HasValue) q = q.Where(o => o.CreatedAt >= from);
            if (to.HasValue) q = q.Where(o => o.CreatedAt <= to);

            if (type.Equals("month", StringComparison.OrdinalIgnoreCase))
            {
                return await q
                    .GroupBy(o => new { o.CreatedAt!.Value.Year, o.CreatedAt!.Value.Month })
                    .Select(g => new
                    {
                        period = $"{g.Key.Year:D4}-{g.Key.Month:D2}",
                        orders = g.Count(),
                        revenue = g.Sum(x => x.TotalAmount)
                    })
                    .OrderBy(x => x.period)
                    .ToListAsync();
            }

            return await q
                .GroupBy(o => o.CreatedAt!.Value.Date)
                .Select(g => new
                {
                    period = g.Key,
                    orders = g.Count(),
                    revenue = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.period)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetTopCustomersAsync(int top)
        {
            return await _db.Orders
                .Where(o => o.PaymentStatus == "PAID" && o.UserId != null)
                .GroupBy(o => o.UserId!)
                .Select(g => new
                {
                    userId = g.Key,
                    totalOrders = g.Count(),
                    totalSpent = g.Sum(x => x.TotalAmount)
                })
                .OrderByDescending(x => x.totalSpent)
                .Take(top)
                .Join(_db.Users, x => x.userId, u => u.Id, (x, u) => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    x.totalOrders,
                    x.totalSpent
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetTopBooksAsync(int top)
        {
            var paidOrderIds = _db.Orders
                .Where(o => o.PaymentStatus == "PAID")
                .Select(o => o.Id);

            return await _db.OrderItems
                .Where(oi => paidOrderIds.Contains(oi.OrderId))
                .GroupBy(oi => oi.BookId)
                .Select(g => new
                {
                    bookId = g.Key,
                    totalQuantity = g.Sum(x => x.Quantity),
                    revenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderByDescending(x => x.totalQuantity)
                .Take(top)
                .Join(_db.Books, x => x.bookId, b => b.Id, (x, b) => new
                {
                    b.Id,
                    b.Title,
                    b.Code,
                    b.Author,
                    x.totalQuantity,
                    x.revenue
                })
                .ToListAsync();
        }

        public async Task<object> GetDeletedItemsAsync()
        {
            var books = await _db.Books.Where(b => (b.IsDeleted ?? false) == true)
                .Select(b => new { b.Id, b.Title, b.Code })
                .ToListAsync();

            var bookImages = await _db.BookImages.Where(i => (i.IsDeleted ?? false) == true)
                .Select(i => new { i.Id, i.BookId, i.ImageUrl })
                .ToListAsync();

            var reviews = await _db.Reviews.Where(r => (r.IsDeleted ?? false) == true)
                .Select(r => new { r.Id, r.BookId, r.UserId, r.Rating })
                .ToListAsync();

            var carts = await _db.Carts.Where(c => (c.IsDeleted ?? false) == true)
                .Select(c => new { c.Id, c.UserId })
                .ToListAsync();

            var categories = await _db.Categories.Where(c => c.IsDeleted)
                .Select(c => new { c.Id, c.CategoryCode, c.CategoryName })
                .ToListAsync();

            var users = await _db.Users.Where(u => u.IsDeleted)
                .Select(u => new { u.Id, u.FullName, u.Email })
                .ToListAsync();

            return new { books, bookImages, reviews, carts, categories, users };
        }

        public async Task<bool> RestoreAsync(string table, long id)
        {
            switch (table.ToLowerInvariant())
            {
                case "books":
                    var b = await _db.Books.FindAsync(id);
                    if (b == null) return false;
                    b.IsDeleted = false;
                    break;
                case "book_images":
                    var bi = await _db.BookImages.FindAsync(id);
                    if (bi == null) return false;
                    bi.IsDeleted = false;
                    break;
                case "reviews":
                    var r = await _db.Reviews.FindAsync(id);
                    if (r == null) return false;
                    r.IsDeleted = false;
                    break;
                case "carts":
                    var c = await _db.Carts.FindAsync(id);
                    if (c == null) return false;
                    c.IsDeleted = false;
                    break;
                case "categories":
                    var cat = await _db.Categories.FindAsync(id);
                    if (cat == null) return false;
                    cat.IsDeleted = false;
                    break;
                default:
                    return false;
            }
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreUserAsync(string userId)
        {
            var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (u == null) return false;
            u.IsDeleted = false;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
