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
    public class GenericService<T>(IGenericRepository<T> repository) : IGenericService<T> where T : class
    {
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var item = await repository.FindById(id);
            return item ?? throw new KeyNotFoundException($"Item {id} not found");
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await repository.Get();
        }

        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T,bool>> predicate)
        {
            var result = await repository.Get(predicate);
            return result ?? throw new KeyNotFoundException("Items not found");
        }

        public virtual async Task<T> CreateAsync(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));


            return await repository.Create(item);
        }

        public virtual async Task<T> UpdateAsync(int id, T item)
        {
            return await repository.Update(id, item);
        }

        public virtual async Task RemoveAsync(T item)
        {
            await repository.Remove(item);
        }
    }

}
