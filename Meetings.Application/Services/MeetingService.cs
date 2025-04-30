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
    public class MeetingService(IMeetingRepository repository, IMeetingValidatorService validator)
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

        public async override Task<Meeting> UpdateAsync(int id, Meeting meeting)
        {
            await validator.ValidASync(meeting, true);
            return await base.UpdateAsync(id, meeting);
        }
    }
}