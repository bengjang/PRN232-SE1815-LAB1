using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Identity.Service.Data;
using PRN232.LMS.Identity.Service.Entities;

namespace PRN232.LMS.Identity.Service.Data;

public static class IdentityDbSeeder
{
    public static async Task SeedAsync(IdentityDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        await context.Users.AddRangeAsync(
        [
            new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Role = "Admin" },
            new User { Username = "teacher", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Role = "Teacher" },
            new User { Username = "student", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Role = "Student" }
        ]);
        await context.SaveChangesAsync();
    }
}
