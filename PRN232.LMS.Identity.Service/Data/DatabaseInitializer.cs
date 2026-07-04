using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Identity.Service.Data;

namespace PRN232.LMS.Identity.Service.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, int maxRetries = 30, int delaySeconds = 3)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<IdentityDbContext>>();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                logger?.LogInformation("Applying identity database migrations (attempt {Attempt}/{Max})...", attempt, maxRetries);
                await db.Database.EnsureCreatedAsync();
                await EnsureSchemaAsync(db);
                await IdentityDbSeeder.SeedAsync(db);
                logger?.LogInformation("Identity database ready.");
                return;
            }
            catch (Exception ex) when (attempt < maxRetries && IsSqlConnectionError(ex))
            {
                logger?.LogWarning("SQL Server not ready (attempt {Attempt}/{Max}). Retrying...", attempt, maxRetries);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }
        }

        throw new InvalidOperationException("Could not connect to Identity SQL Server.");
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

    private static async Task EnsureSchemaAsync(IdentityDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync("""
IF OBJECT_ID(N'[Users]', N'U') IS NULL
BEGIN
    CREATE TABLE [Users] (
        [UserId] int NOT NULL IDENTITY,
        [Username] nvarchar(100) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [Role] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
    );
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;

IF OBJECT_ID(N'[RefreshTokens]', N'U') IS NULL
BEGIN
    CREATE TABLE [RefreshTokens] (
        [RefreshTokenId] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Token] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [IsRevoked] bit NOT NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([RefreshTokenId]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
    CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;
""");
    }
}
