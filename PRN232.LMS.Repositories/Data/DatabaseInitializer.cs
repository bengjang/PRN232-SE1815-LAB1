using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PRN232.LMS.Repositories.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        int maxRetries = 30,
        int delaySeconds = 3)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<LmsDbContext>>();
        var db = scope.ServiceProvider.GetRequiredService<LmsDbContext>();

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                logger?.LogInformation("Applying database migrations (attempt {Attempt}/{Max})...", attempt, maxRetries);
                await db.Database.MigrateAsync();
                await DbSeeder.SeedAsync(db);
                logger?.LogInformation("Database ready.");
                return;
            }
            catch (Exception ex) when (attempt < maxRetries && IsSqlConnectionError(ex))
            {
                logger?.LogWarning(
                    "SQL Server not ready yet (attempt {Attempt}/{Max}). Retrying in {Delay}s...",
                    attempt, maxRetries, delaySeconds);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }
        }

        throw new InvalidOperationException(
            "Could not connect to SQL Server. Start Docker Desktop, then run: docker compose up --build");
    }

    private static bool IsSqlConnectionError(Exception ex)
    {
        for (var current = ex; current != null; current = current.InnerException)
        {
            if (current is SqlException sql &&
                (sql.Number == -2 || sql.Number == 2 || sql.Number == 53 || sql.Number == 4060 || sql.Number == 18456))
                return true;
        }

        return ex is TimeoutException or InvalidOperationException;
    }
}

