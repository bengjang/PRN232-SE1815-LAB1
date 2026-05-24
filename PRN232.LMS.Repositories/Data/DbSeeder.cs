using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data;

public static class DbSeeder
{
    private static readonly string[] EnrollmentStatuses = ["Active", "Completed", "Dropped", "Pending"];

    public static async Task SeedAsync(LmsDbContext context)
    {
        if (await context.Students.AnyAsync())
            return;

        var semesters = CreateSemesters();
        await context.Semesters.AddRangeAsync(semesters);
        await context.SaveChangesAsync();

        var subjects = CreateSubjects();
        await context.Subjects.AddRangeAsync(subjects);
        await context.SaveChangesAsync();

        var courses = CreateCourses(semesters);
        await context.Courses.AddRangeAsync(courses);
        await context.SaveChangesAsync();

        var students = CreateStudents();
        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();

        var enrollments = CreateEnrollments(students, courses);
        await context.Enrollments.AddRangeAsync(enrollments);
        await context.SaveChangesAsync();
    }

    private static List<Semester> CreateSemesters() =>
    [
        new() { SemesterName = "Spring 2023", StartDate = new DateTime(2023, 1, 15), EndDate = new DateTime(2023, 5, 15) },
        new() { SemesterName = "Summer 2023", StartDate = new DateTime(2023, 6, 1), EndDate = new DateTime(2023, 8, 31) },
        new() { SemesterName = "Fall 2023", StartDate = new DateTime(2023, 9, 1), EndDate = new DateTime(2023, 12, 20) },
        new() { SemesterName = "Spring 2024", StartDate = new DateTime(2024, 1, 15), EndDate = new DateTime(2024, 5, 15) },
        new() { SemesterName = "Fall 2024", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 12, 20) }
    ];

    private static List<Subject> CreateSubjects() =>
    [
        new() { SubjectCode = "PRN232", SubjectName = "Building Cross-Platform Back-End Application", Credit = 3 },
        new() { SubjectCode = "SWP391", SubjectName = "Software Development Project", Credit = 5 },
        new() { SubjectCode = "DBI202", SubjectName = "Introduction to Database Systems", Credit = 3 },
        new() { SubjectCode = "MAE101", SubjectName = "Mathematics for Engineering", Credit = 3 },
        new() { SubjectCode = "OSG202", SubjectName = "Operating Systems", Credit = 3 },
        new() { SubjectCode = "JPD113", SubjectName = "Elementary Japanese 1", Credit = 3 },
        new() { SubjectCode = "SSL101", SubjectName = "Soft Skills and Teamwork", Credit = 2 },
        new() { SubjectCode = "NWC203", SubjectName = "Computer Networking", Credit = 3 },
        new() { SubjectCode = "ITE302", SubjectName = "Information Technology Ethics", Credit = 2 },
        new() { SubjectCode = "PMG201", SubjectName = "Project Management Fundamentals", Credit = 3 }
    ];

    private static List<Course> CreateCourses(List<Semester> semesters)
    {
        var courseTemplates = new (string Name, int SemesterIndex)[]
        {
            ("PRN232 - Section A", 0), ("PRN232 - Section B", 1),
            ("SWP391 - Capstone A", 2), ("SWP391 - Capstone B", 3),
            ("DBI202 - Fundamentals", 0), ("DBI202 - Advanced Lab", 4),
            ("MAE101 - Morning", 1), ("MAE101 - Evening", 2),
            ("OSG202 - Linux Track", 3), ("OSG202 - Windows Track", 4),
            ("JPD113 - Beginner", 0), ("SSL101 - Workshop", 1),
            ("NWC203 - Routing", 2), ("NWC203 - Switching", 3),
            ("ITE302 - Ethics Seminar", 4), ("PMG201 - Agile", 0),
            ("PRN232 - Intensive", 2), ("SWP391 - Industry", 4),
            ("DBI202 - SQL Mastery", 3), ("MAE101 - Remedial", 1)
        };

        return courseTemplates.Select(t => new Course
        {
            CourseName = t.Name,
            SemesterId = semesters[t.SemesterIndex].SemesterId
        }).ToList();
    }

    private static List<Student> CreateStudents()
    {
        var firstNames = new[] { "Nguyen", "Tran", "Le", "Pham", "Hoang", "Phan", "Vu", "Dang", "Bui", "Do" };
        var lastNames = new[] { "Van An", "Thi Binh", "Minh Chau", "Gia Dat", "Thanh Em", "Quoc Huy", "Kim Lan", "Xuan Mai", "Duc Nam", "Thu Oanh" };
        var students = new List<Student>();
        var random = new Random(42);

        for (var i = 1; i <= 50; i++)
        {
            var first = firstNames[(i - 1) % firstNames.Length];
            var last = lastNames[(i - 1) % lastNames.Length];
            var fullName = $"{first} {last}";
            students.Add(new Student
            {
                FullName = fullName,
                Email = $"student{i:D2}@fpt.edu.vn",
                DateOfBirth = new DateTime(2000 + random.Next(0, 5), random.Next(1, 13), random.Next(1, 28))
            });
        }

        return students;
    }

    private static List<Enrollment> CreateEnrollments(List<Student> students, List<Course> courses)
    {
        var random = new Random(42);
        var enrollments = new List<Enrollment>();
        var usedPairs = new HashSet<(int StudentId, int CourseId)>();

        while (enrollments.Count < 500)
        {
            var student = students[random.Next(students.Count)];
            var course = courses[random.Next(courses.Count)];
            var pair = (student.StudentId, course.CourseId);

            if (!usedPairs.Add(pair))
                continue;

            var semester = course.SemesterId;
            var enrollDate = DateTime.UtcNow.Date.AddDays(-random.Next(30, 400));

            enrollments.Add(new Enrollment
            {
                StudentId = student.StudentId,
                CourseId = course.CourseId,
                EnrollDate = enrollDate,
                Status = EnrollmentStatuses[random.Next(EnrollmentStatuses.Length)]
            });
        }

        return enrollments;
    }
}

