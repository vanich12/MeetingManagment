using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Meetings.Core.Contexts;
using Meetings.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Meetings.Infrastructure.Storages
{
    /// <summary>
    /// Универсальный репозиторий
    /// </summary>
    /// <typeparam name="T"> Сущность с которой работает репозиторий</typeparam>
    /// <param name="context"></param>
    public class GenericRepository<T>(MeetingAppContext context): IGenericRepository<T> where T : class
    {
        private readonly MeetingAppContext _ctx = context;
        private readonly DbSet<T> _dbSet = context.Set<T>();

        /// <summary>
        /// Создание сущности
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<T> Create(T item)
        {
            this._dbSet.Add(item);
            await this._ctx.SaveChangesAsync();
            return item;
        }
        /// <summary>
        /// Поиск сущности по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T?> FindById(int id)
        {
            return await this._dbSet.FindAsync(id);
        }
        /// <summary>
        /// Получение всего списка сущностей
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> Get()
        {
            return await this._dbSet.AsNoTracking().ToListAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        /// <summary>
        /// Проверка наличия сущностей 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter); 
        }
        /// <summary>
        /// Полученеи сущности по предикату
        /// </summary>
        /// <param name="predicate"> предикат</param>
        /// <returns></returns>
        public async Task<T?> GetOne(Predicate<T> predicate)
        {
            await foreach (var item in this._dbSet.AsAsyncEnumerable())
            {
                if (predicate(item))
                    return item;
            }

            return null;
        }
        /// <summary>
        /// Удаление сущности
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task Remove(T item)
        {
            this._dbSet.Remove(item);
            await this._ctx.SaveChangesAsync();
        }
        /// <summary>
        /// Обновление сущности
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<T?> Update(int id, T item)
        {
            var oldItem = await this._dbSet.FindAsync(id);
            if (oldItem == null)
            {
                return null;
            }

            item.GetType().GetProperty("Id")?.SetValue(item, id);
            this._ctx.Entry(oldItem).CurrentValues.SetValues(item);
            await this._ctx.SaveChangesAsync();
            return item;
        }
    }
}
