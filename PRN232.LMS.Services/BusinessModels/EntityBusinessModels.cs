namespace PRN232.LMS.Services.BusinessModels;

public class StudentBusinessModel
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public List<EnrollmentBusinessModel>? Enrollments { get; set; }
}

public class SemesterBusinessModel
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<CourseBusinessModel>? Courses { get; set; }
}

public class CourseBusinessModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public int SemesterId { get; set; }
    public SemesterBusinessModel? Semester { get; set; }
    public List<EnrollmentBusinessModel>? Enrollments { get; set; }
}

public class SubjectBusinessModel
{
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = null!;
    public string SubjectName { get; set; } = null!;
    public int Credit { get; set; }
}

public class EnrollmentBusinessModel
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = null!;
    public StudentBusinessModel? Student { get; set; }
    public CourseBusinessModel? Course { get; set; }
}

