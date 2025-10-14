using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PROJECT_BOOK_STORE_GROUP5_PRN222.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        // Lấy tất cả
        Task<IEnumerable<T>> GetAllAsync();

        // Lấy theo Id
        Task<T?> GetByIdAsync(long id);

        // Tìm với điều kiện (predicate)
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Thêm mới
        Task AddAsync(T entity);

        // Thêm nhiều
        Task AddRangeAsync(IEnumerable<T> entities);

        // Cập nhật
        Task UpdateAsync(T entity);

        // Xoá theo entity
        Task DeleteAsync(T entity);

        // Xoá theo Id
        Task DeleteByIdAsync(long id);
    }
}
