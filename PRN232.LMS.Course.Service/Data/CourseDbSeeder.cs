using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Course.Service.Entities;
using CourseEntity = PRN232.LMS.Course.Service.Entities.Course;

namespace PRN232.LMS.Course.Service.Data;

public static class CourseDbSeeder
{
    private static readonly string[] EnrollmentStatuses = ["Active", "Completed", "Dropped", "Pending"];

    public static async Task SeedAsync(CourseDbContext context)
    {
        if (await context.Courses.AnyAsync())
            return;

        var semesters = new List<Semester>
        {
            new() { SemesterName = "Spring 2024", StartDate = new DateTime(2024, 1, 15), EndDate = new DateTime(2024, 5, 15) },
            new() { SemesterName = "Fall 2024", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 12, 20) }
        };
        await context.Semesters.AddRangeAsync(semesters);
        await context.SaveChangesAsync();

        var courses = new List<CourseEntity>
        {
            new() { CourseName = "PRN232 - Section A", SemesterId = semesters[0].SemesterId },
            new() { CourseName = "PRN232 - Section B", SemesterId = semesters[0].SemesterId },
            new() { CourseName = "SWP391 - Capstone", SemesterId = semesters[1].SemesterId },
            new() { CourseName = "DBI202 - Fundamentals", SemesterId = semesters[1].SemesterId }
        };
        await context.Courses.AddRangeAsync(courses);
        await context.SaveChangesAsync();

        var random = new Random(42);
        var enrollments = new List<Enrollment>();
        for (var studentId = 1; studentId <= 10; studentId++)
        {
            var course = courses[random.Next(courses.Count)];
            enrollments.Add(new Enrollment
            {
                StudentId = studentId,
                CourseId = course.CourseId,
                EnrollDate = DateTime.UtcNow.Date.AddDays(-random.Next(1, 30)),
                Status = EnrollmentStatuses[random.Next(EnrollmentStatuses.Length)]
            });
        }

        await context.Enrollments.AddRangeAsync(enrollments);
        await context.SaveChangesAsync();
    }
}
