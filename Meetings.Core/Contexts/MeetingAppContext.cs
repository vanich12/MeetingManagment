using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meetings.Core.Configuration;
using Meetings.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Meetings.Core.Contexts
{
    public abstract class MeetingAppContext : DbContext
    {
        public DbSet<Meeting> Meetings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            MakeModelsConfiguration(modelBuilder);
        }

        private static void MakeModelsConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MeetingConfiguration());
        }
    }
}