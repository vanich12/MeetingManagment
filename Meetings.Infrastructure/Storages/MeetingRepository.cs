using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meetings.Core.Contexts;
using Meetings.Core.Models;
using Meetings.Infrastructure.DTO;
using Meetings.Infrastructure.Filters;
using Meetings.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Meetings.Infrastructure.Storages
{
    public class MeetingRepository(MeetingAppContext context) : GenericRepository<Meeting>(context), IMeetingRepository
    {
        private readonly MeetingAppContext _ctx = context;
        public async Task<IEnumerable<Meeting>> GetMeetingsByParams(MeetingFilterDTO filters)
        {
            var query = this._ctx.Meetings.ApplyFilters(filters);

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
