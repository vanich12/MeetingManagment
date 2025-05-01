using System.Linq.Expressions;

namespace Meetings.Application.Services.Interfaces
{
    public interface IGenericService<T>
    {
        /// <summary>
        /// Получение по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Получить все
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();
        /// <summary>
        /// Получить по предикату
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
            /// <summary>
            /// Создать
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
        Task<T> CreateAsync(T item);
        /// <summary>
        /// Обновить сущность
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<T> UpdateAsync(int id, T item);
        /// <summary>
        /// Удалить сущность
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task RemoveAsync(T item);
    }
}
