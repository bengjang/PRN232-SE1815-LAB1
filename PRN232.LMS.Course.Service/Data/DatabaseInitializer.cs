using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Course.Service.Data;

namespace PRN232.LMS.Course.Service.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, int maxRetries = 30, int delaySeconds = 3)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<CourseDbContext>>();
        var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                logger?.LogInformation("Applying course database migrations (attempt {Attempt}/{Max})...", attempt, maxRetries);
                await db.Database.EnsureCreatedAsync();
                await EnsureSchemaAsync(db);
                await CourseDbSeeder.SeedAsync(db);
                logger?.LogInformation("Course database ready.");
                return;
            }
            catch (Exception ex) when (attempt < maxRetries && IsSqlConnectionError(ex))
            {
                logger?.LogWarning("SQL Server not ready (attempt {Attempt}/{Max}). Retrying...", attempt, maxRetries);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }
        }

        throw new InvalidOperationException("Could not connect to Course SQL Server.");
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

    private static async Task EnsureSchemaAsync(CourseDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync("""
IF OBJECT_ID(N'[Semesters]', N'U') IS NULL
BEGIN
    CREATE TABLE [Semesters] (
        [SemesterId] int NOT NULL IDENTITY,
        [SemesterName] nvarchar(100) NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Semesters] PRIMARY KEY ([SemesterId])
    );
END;

IF OBJECT_ID(N'[Courses]', N'U') IS NULL
BEGIN
    CREATE TABLE [Courses] (
        [CourseId] int NOT NULL IDENTITY,
        [CourseName] nvarchar(200) NOT NULL,
        [SemesterId] int NOT NULL,
        CONSTRAINT [PK_Courses] PRIMARY KEY ([CourseId]),
        CONSTRAINT [FK_Courses_Semesters_SemesterId] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters] ([SemesterId]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_Courses_SemesterId] ON [Courses] ([SemesterId]);
END;

IF OBJECT_ID(N'[Enrollments]', N'U') IS NULL
BEGIN
    CREATE TABLE [Enrollments] (
        [EnrollmentId] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [CourseId] int NOT NULL,
        [EnrollDate] datetime2 NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Enrollments] PRIMARY KEY ([EnrollmentId]),
        CONSTRAINT [FK_Enrollments_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([CourseId]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_Enrollments_CourseId] ON [Enrollments] ([CourseId]);
    CREATE UNIQUE INDEX [IX_Enrollments_StudentId_CourseId] ON [Enrollments] ([StudentId], [CourseId]);
END;
""");
    }
}
