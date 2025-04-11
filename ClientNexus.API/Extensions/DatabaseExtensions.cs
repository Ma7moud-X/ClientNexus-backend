using ClientNexus.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ClientNexus.API.Extensions;

public static class DatabaseExtensions
{
    //public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    //{
    //    var connectionStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STR");
    //    if (connectionStr is null)
    //    {
    //        throw new Exception("Database connection string environment variable is not set");
    //    }

    //    // Console.WriteLine(connectionStr);

    //    services.AddDbContext<ApplicationDbContext>(options =>
    //    {
    //        options.UseSqlServer(
    //            connectionStr,
    //            sqlOptions =>
    //            {
    //                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
    //            }
    //        );
    //    });
    //}


    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStr = configuration.GetConnectionString("DB_CONNECTION_STR");
        if (connectionStr is null)
        {
            throw new Exception("Database connection string is not configured");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                connectionStr,
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                }
            );
        });
    }


}
