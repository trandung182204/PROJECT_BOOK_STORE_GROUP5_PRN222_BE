namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public interface IAdminRepository
    {
        Task<object> GetSummaryAsync();
        Task<IEnumerable<object>> GetSalesReportAsync(string type, DateTime? from, DateTime? to);
        Task<IEnumerable<object>> GetTopCustomersAsync(int top);
        Task<IEnumerable<object>> GetTopBooksAsync(int top);
        Task<object> GetDeletedItemsAsync();
        Task<bool> RestoreAsync(string table, long id);
        Task<bool> RestoreUserAsync(string userId);
    }
}
