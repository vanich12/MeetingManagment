using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Meetings.Core.Contexts
{
    public class PGContext : MeetingAppContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql($"Host=localhost;Port=5432;Database=MeetingsAppDB;Username=postgres;Password=123;");
#if DEBUG
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
#endif
        }

        public PGContext()
        {
            //this.Database.EnsureDeleted();
            //this.Database.EnsureCreated();
        }
    }
}
