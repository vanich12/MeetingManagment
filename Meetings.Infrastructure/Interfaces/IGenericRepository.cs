using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Meetings.Infrastructure.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Создание.
        /// </summary>
        /// <param name="item">Объект.</param>
        /// <returns>Объект.</returns>
        Task<T> Create(T item);

        /// <summary>
        /// Поиск объекта по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <returns>Объект.</returns>
        Task<T?> FindById(int id);

        /// <summary>
        /// Список объектов.
        /// </summary>
        /// <returns>Список объектов.</returns>
        Task<IEnumerable<T>> Get();

        /// <summary>
        /// Список объектов, с указанным условием (используем Expression, так как EF стролит дерево выражений перед запросом).
        /// </summary>
        /// <param name="predicate">Условие.</param>
        /// <returns>Список объектов, с указанным условием.</returns>
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Получение подходящей сущности.
        /// </summary>
        /// <param name="predicate">Функция, по условию которой производится отбор данных из БД.</param>
        /// <returns>Сущность.</returns>
        Task<T?> GetOne(Predicate<T> predicate);

        /// <summary>
        /// Удаление объекта.
        /// </summary>
        /// <param name="item">Объект.</param>
        /// <returns>Результат удаления.</returns>
        Task Remove(T item);

        /// <summary>
        /// Изменение объекта.
        /// </summary>
        /// <param name="id">Идентификатор объекта.</param>
        /// <param name="item">Объект.</param>
        /// <returns>Объект.</returns>
        Task<T?> Update(int id, T item);

        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
    }
}
