using System.Linq.Expressions;

namespace Meetings.Application.Services.Interfaces
{
    public interface IGenericService<T>
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<T> CreateAsync(T item);
        Task<T> UpdateAsync(int id, T item);
        Task RemoveAsync(T item);
    }
}
