using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meetings.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Meetings.Core.Configuration
{
    public class MeetingConfiguration: IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
