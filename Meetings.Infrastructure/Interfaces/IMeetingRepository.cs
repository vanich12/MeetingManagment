using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meetings.Core.Models;
using Meetings.Infrastructure.DTO;
using Meetings.Infrastructure.Filters;

namespace Meetings.Infrastructure.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория встреч
    /// </summary>
    public interface IMeetingRepository : IGenericRepository<Meeting>
    {
        /// <summary>
        /// фильтрация встреч
        /// </summary>
        /// <param name="filters"> фильтры </param>
        /// <returns></returns>
        Task<IEnumerable<Meeting>> GetMeetingsByParams(MeetingFilterDTO filters);
    }
}
