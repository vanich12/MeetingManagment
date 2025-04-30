using MeetingManagment;
using MeetingManagment.HostBuilder;
using Meetings.Application.Services.Interfaces;
using Meetings.Application.Services;
using Meetings.Infrastructure.Interfaces;
using Meetings.Infrastructure.Storages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Meetings.Core.Contexts;
using Serilog;

public class Program
{
    public static async Task Main(string[] args) // Сделаем Main асинхронным
    {
        var host = Host.CreateDefaultBuilder(args)
            .BuildConfiguration()
            .BuildLogging()
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<MeetingAppContext, PGContext>();
                services.AddScoped<PGContext>();
                services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
                services.AddScoped<IMeetingRepository, MeetingRepository>();
                services.AddScoped<IMeetingService, MeetingService>();
                services.AddScoped<IMeetingValidatorService, MeetingValidatorService>();
                services.AddTransient<MeetingHandler>();
            })
            .Build();

        using (var serviceScope = host.Services.CreateScope())
        {
            var services = serviceScope.ServiceProvider;

            try
            {
                var app = services.GetRequiredService<MeetingHandler>();
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}