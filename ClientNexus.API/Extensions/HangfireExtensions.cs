using ClientNexus.Application.Interfaces;
using Hangfire;

namespace ClientNexus.API.Extensions
{
    public static class HangfireExtensions
    {
        public static void AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
        {
            var hangfireConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STR");

            if (string.IsNullOrEmpty(hangfireConnectionString))
            {
                throw new InvalidOperationException("Hangfire connection string is not configured");
            }

            services.AddHangfire(config => config.UseSqlServerStorage(hangfireConnectionString));

            services.AddHangfireServer();

        }
        public static void UseHangfireConfiguration(this IApplicationBuilder app)
        {
            // Configure dashboard
            app.UseHangfireDashboard();

            // Register recurring jobs
            ConfigureRecurringJobs();

        }
        private static void ConfigureRecurringJobs()
        {
            // Schedule appointment reminders
            RecurringJob.AddOrUpdate<IAppointmentService>(
                "appointment-reminders",
                service => service.SendAppointmentReminderAsync(),
                Cron.MinuteInterval(10));

            // more recurring jobs
        }
    }
}
