using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Meetings.Application.Extensions;
using Meetings.Application.Services.Interfaces;
using Meetings.Core.Models;
using Meetings.Infrastructure.Interfaces;

namespace Meetings.Application.Services
{
    /// <summary>
    /// Серви встреч
    /// </summary>
    /// <param name="repository"></param>
    public class MeetingService(IMeetingRepository repository):GenericService<Meeting>(repository), IMeetingService
    {
        /// <summary>
        /// Валидация встречи перед созданием
        /// </summary>
        /// <param name="meeting"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async override Task<Meeting> CreateAsync(Meeting meeting)
        {
            if (meeting == null)
                throw new ArgumentException(nameof(meeting));

            if (meeting.StartTime < DateTime.Now.ToUtcSafe())
                throw new Exception("Встреча не может быть назначена на время, которое прошло");

            if (meeting.StartTime > meeting.EndTime)
                throw new Exception("Окончанеи встречи не может быть раньше начала");

            if (meeting.Reminder > meeting.StartTime)
                throw new Exception("Время напоминания о встрече не может быть раньше встречи");

            Expression<Func<Meeting, bool>> overlapFilter = existingMeeting =>
                meeting.StartTime < existingMeeting.EndTime && meeting.EndTime > existingMeeting.StartTime;

            var overlaps = await repository.AnyAsync(overlapFilter);

            if (overlaps)
                throw new Exception($"Встреча уже назначена на период: {meeting.StartTime} - {meeting.EndTime}.");
            

            return await base.CreateAsync(meeting);
        }
    }
}
