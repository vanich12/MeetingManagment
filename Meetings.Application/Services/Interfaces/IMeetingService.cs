using Meetings.Core.Models;

namespace Meetings.Application.Services.Interfaces
{
    public interface IMeetingService:IGenericService<Meeting>
    {
        Task CheckReminders();
    }
}
