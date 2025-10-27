namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Services
{
    public interface IAdminService
    {
        Task<object> GetSummaryAsync();
        Task<IEnumerable<object>> GetSalesReportAsync(string type = "day", DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<object>> GetTopCustomersAsync(int top = 10);
        Task<IEnumerable<object>> GetTopBooksAsync(int top = 10);
        Task<object> GetDeletedItemsAsync();
        Task<bool> RestoreAsync(string table, long id);
        Task<bool> RestoreUserAsync(string userId);
    }
}
