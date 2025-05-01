using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meetings.Infrastructure.DTO
{
    /// <summary>
    /// Поля для фильтрации
    /// </summary>
    public class MeetingFilterDTO
    {
        public DateTime? StartDate { get; set; }
    }
}
