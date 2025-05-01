using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Meetings.Application.Services.Interfaces;
using Meetings.Infrastructure.Interfaces;

namespace Meetings.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repository"></param>
    public class GenericService<T>(IGenericRepository<T> repository) : IGenericService<T> where T : class
    {
        /// <summary>
        /// Получить сущность по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var item = await repository.FindById(id);
            return item ?? throw new KeyNotFoundException($"Item {id} not found");
        }
        /// <summary>
        /// Получить список всех сущностей
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await repository.Get();
        }

        /// <summary>
        /// Получить сущности по предикату
        /// </summary>
        /// <param name="predicate"> предикат </param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T,bool>> predicate)
        {
            var result = await repository.Get(predicate);
            return result ?? throw new KeyNotFoundException("Items not found");
        }

        /// <summary>
        /// создать сущность
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<T> CreateAsync(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));


            return await repository.Create(item);
        }
        /// <summary>
        /// Обновить сущность
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual async Task<T> UpdateAsync(int id, T item)
        {
            return await repository.Update(id, item);
        }

        /// <summary>
        /// Удалить сущность
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(T item)
        {
            await repository.Remove(item);
        }
    }

}
