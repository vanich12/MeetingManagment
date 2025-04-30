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
using Serilog;

namespace Meetings.Application.Services
{
    /// <summary>
    /// Серви встреч
    /// </summary>
    /// <param name="repository"></param>
    public class MeetingService(IMeetingRepository repository, IMeetingValidatorService validator, ILogger logger)
        : GenericService<Meeting>(repository), IMeetingService
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
            await validator.ValidASync(meeting);
            return await base.CreateAsync(meeting);
        }

        /// <summary>
        /// Обновление встречи
        /// </summary>
        /// <param name="id"></param>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public async override Task<Meeting> UpdateAsync(int id, Meeting meeting)
        {
            await validator.ValidASync(meeting, true);
            return await base.UpdateAsync(id, meeting);
        }


        public async Task CheckReminders()
        {
            Expression<Func<Meeting, bool>> expression = meeting =>
                meeting.Reminder <= DateTime.UtcNow && meeting.Reminder != null;
            var meetings = await repository.Get(expression);
            foreach (var meeting in meetings)
            {
                Console.WriteLine(
                    $"\n Напоминание: У вас назначена встреча на {meeting.StartTime.FormatForDisplay()}");
                meeting.Reminder = null;
            }

            foreach (var meeting in meetings)
            {
                try
                {
                    meeting.Reminder = null;
                    await repository.Update(meeting.Id, meeting);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Ошибка обработки/обновления напоминания для встречи ID: {MeetingId}", meeting.Id);
                }
            }
        }
    }
}