using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meetings.Core.Models;
using Meetings.Infrastructure.DTO;
using Microsoft.EntityFrameworkCore;

namespace Meetings.Infrastructure.Filters
{

    public static class MeetingExtension
    {
        // сделано как пример, есть возможность добавить фильтры (например для пагинации)
        public static IQueryable<Meeting> ApplyFilters(this IQueryable<Meeting> query, MeetingFilterDTO filters)
        {
            if (filters is null)
                return query;

            if (filters.StartDate.HasValue)
                query = query.AsNoTracking().Where(m => m.StartTime <= filters.StartDate);

            return query;
        }
    }
}
