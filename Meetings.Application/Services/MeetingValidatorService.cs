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
    /// Бизнес-правила валидации встречи
    /// </summary>
    /// <param name="repository"></param>
    public class MeetingValidatorService(IMeetingRepository repository) : IMeetingValidatorService
    {
        /// <summary>
        /// Валидация встреч
        /// </summary>
        /// <param name="meeting">встреча</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task ValidASync(Meeting meeting, bool isUpdate = false)
        {
            if (meeting == null)
                throw new ArgumentException(nameof(meeting));

            if (meeting.StartTime < DateTime.Now.ToUtcSafe())
                throw new Exception("Встреча не может быть назначена на время, которое прошло");

            if (meeting.StartTime > meeting.EndTime)
                throw new Exception("Окончанеи встречи не может быть раньше начала");

            if (meeting.Reminder > meeting.StartTime)
                throw new Exception("Время напоминания о встрече не может быть раньше встречи");

            if (meeting.Reminder < DateTime.Now.ToUtcSafe())
                throw new Exception("Время напоминания о встрече не может быть назначено на прошедшее время");

            Expression<Func<Meeting, bool>> overlapFilter = existingMeeting =>
                meeting.StartTime < existingMeeting.EndTime && meeting.EndTime > existingMeeting.StartTime;

            if (!isUpdate)
            {
                var overlaps = await repository.AnyAsync(overlapFilter);

                if (overlaps)
                    throw new Exception($"Встреча уже назначена на период: {meeting.StartTime} - {meeting.EndTime}.");
            }
        }
    }
}