using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MeetingManagment.HostBuilder
{
    public static class BuildLoggingExtension
    {
        public static IHostBuilder BuildLogging(this IHostBuilder builder) => builder.ConfigureServices((context,
            services) =>
        {
            services.AddSerilog((_, loggerConfiguration) =>
                loggerConfiguration.ReadFrom.Configuration(context.Configuration));
        });
    }
}