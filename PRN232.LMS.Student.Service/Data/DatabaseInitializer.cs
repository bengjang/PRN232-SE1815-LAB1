using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Student.Service.Data;

namespace PRN232.LMS.Student.Service.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, int maxRetries = 30, int delaySeconds = 3)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<StudentDbContext>>();
        var db = scope.ServiceProvider.GetRequiredService<StudentDbContext>();

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                logger?.LogInformation("Applying student database migrations (attempt {Attempt}/{Max})...", attempt, maxRetries);
                await db.Database.EnsureCreatedAsync();
                await EnsureSchemaAsync(db);
                await StudentDbSeeder.SeedAsync(db);
                logger?.LogInformation("Student database ready.");
                return;
            }
            catch (Exception ex) when (attempt < maxRetries && IsSqlConnectionError(ex))
            {
                logger?.LogWarning("SQL Server not ready (attempt {Attempt}/{Max}). Retrying...", attempt, maxRetries);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }
        }

        throw new InvalidOperationException("Could not connect to Student SQL Server.");
    }

    private static bool IsSqlConnectionError(Exception ex)
    {
        for (var current = ex; current != null; current = current.InnerException)
        {
            if (current is SqlException sql &&
                (sql.Number == -2 || sql.Number == 2 || sql.Number == 53 || sql.Number == 1801 || sql.Number == 4060 || sql.Number == 18456))
                return true;
        }
        return ex is TimeoutException or InvalidOperationException;
    }

    private static async Task EnsureSchemaAsync(StudentDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync("""
IF OBJECT_ID(N'[Students]', N'U') IS NULL
BEGIN
    CREATE TABLE [Students] (
        [StudentId] int NOT NULL IDENTITY,
        [FullName] nvarchar(200) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [DateOfBirth] datetime2 NOT NULL,
        CONSTRAINT [PK_Students] PRIMARY KEY ([StudentId])
    );
    CREATE UNIQUE INDEX [IX_Students_Email] ON [Students] ([Email]);
END;
""");
    }
}
