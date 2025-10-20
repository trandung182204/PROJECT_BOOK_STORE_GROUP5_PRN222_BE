using PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repo;
        public AdminService(IAdminRepository repo) => _repo = repo;

        public Task<object> GetSummaryAsync() => _repo.GetSummaryAsync();

        public Task<IEnumerable<object>> GetSalesReportAsync(string type = "day", DateTime? from = null, DateTime? to = null)
            => _repo.GetSalesReportAsync(type, from, to);

        public Task<IEnumerable<object>> GetTopCustomersAsync(int top = 10)
            => _repo.GetTopCustomersAsync(top);

        public Task<IEnumerable<object>> GetTopBooksAsync(int top = 10)
            => _repo.GetTopBooksAsync(top);

        public Task<object> GetDeletedItemsAsync() => _repo.GetDeletedItemsAsync();

        public Task<bool> RestoreAsync(string table, long id) => _repo.RestoreAsync(table, id);

        public Task<bool> RestoreUserAsync(string userId) => _repo.RestoreUserAsync(userId);
    }
}
