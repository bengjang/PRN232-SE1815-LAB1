using Microsoft.EntityFrameworkCore;
using StudentEntity = PRN232.LMS.Student.Service.Entities.Student;

namespace PRN232.LMS.Student.Service.Data;

public static class StudentDbSeeder
{
    public static async Task SeedAsync(StudentDbContext context)
    {
        if (await context.Students.AnyAsync())
            return;

        var firstNames = new[] { "Nguyen", "Tran", "Le", "Pham", "Hoang", "Phan", "Vu", "Dang", "Bui", "Do" };
        var lastNames = new[] { "Van An", "Thi Binh", "Minh Chau", "Gia Dat", "Thanh Em", "Quoc Huy", "Kim Lan", "Xuan Mai", "Duc Nam", "Thu Oanh" };
        var random = new Random(42);
        var students = new List<StudentEntity>();

        for (var i = 1; i <= 50; i++)
        {
            students.Add(new StudentEntity
            {
                FullName = $"{firstNames[(i - 1) % firstNames.Length]} {lastNames[(i - 1) % lastNames.Length]}",
                Email = $"student{i:D2}@fpt.edu.vn",
                DateOfBirth = new DateTime(2000 + random.Next(0, 5), random.Next(1, 13), random.Next(1, 28))
            });
        }

        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();
    }
}
