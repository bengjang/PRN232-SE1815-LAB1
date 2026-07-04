namespace PRN232.LMS.Course.Service.Models;

public class CourseDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public int SemesterId { get; set; }
    public string? SemesterName { get; set; }
}

public class EnrollmentDto
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = null!;
    public string? StudentFullName { get; set; }
    public string? CourseName { get; set; }
}

public class CreateCourseRequest
{
    public string CourseName { get; set; } = null!;
    public int SemesterId { get; set; }
}

public class EnrollStudentRequest
{
    public int StudentId { get; set; }
    public string Status { get; set; } = "Active";
}
