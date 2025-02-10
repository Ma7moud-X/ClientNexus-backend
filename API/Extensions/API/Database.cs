using Database;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions.API;

public static class Database
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
        var dbUser = Environment.GetEnvironmentVariable("DB_USER");
        var dbPass = Environment.GetEnvironmentVariable("DB_PASS");
        if (dbServer is null || dbUser is null || dbPass is null)
        {
            throw new Exception("Database environment variables are not set");
        }

        string? connectionStrTemplate = configuration.GetConnectionString("DefaultConnection");
        if (connectionStrTemplate is null)
        {
            throw new Exception("Connection string is null");
        }

        string connectionStr = connectionStrTemplate
            .Replace("{DB_SERVER}", dbServer)
            .Replace("{DB_USER}", dbUser)
            .Replace("{DB_PASS}", dbPass);

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
