using Meetings.Core.Models;

namespace Meetings.Application.Services.Interfaces
{
    public interface IMeetingService:IGenericService<Meeting>
    {
        /// <summary>
        /// Проверка уведомлений
        /// </summary>
        /// <returns></returns>
        Task CheckReminders();
    }
}
