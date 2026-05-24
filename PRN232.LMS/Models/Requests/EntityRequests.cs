using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class CreateStudentRequest
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = null!;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    public DateTime DateOfBirth { get; set; }
}

public class UpdateStudentRequest : CreateStudentRequest;

public class CreateSemesterRequest
{
    [Required, MaxLength(100)]
    public string SemesterName { get; set; } = null!;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }
}

public class UpdateSemesterRequest : CreateSemesterRequest;

public class CreateCourseRequest
{
    [Required, MaxLength(100)]
    public string CourseName { get; set; } = null!;

    [Required]
    public int SemesterId { get; set; }
}

public class UpdateCourseRequest : CreateCourseRequest;

public class CreateSubjectRequest
{
    [Required, MaxLength(20)]
    public string SubjectCode { get; set; } = null!;

    [Required, MaxLength(100)]
    public string SubjectName { get; set; } = null!;

    [Required, Range(1, 20)]
    public int Credit { get; set; }
}

public class UpdateSubjectRequest : CreateSubjectRequest;

public class CreateEnrollmentRequest
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    public DateTime EnrollDate { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = null!;
}

public class UpdateEnrollmentRequest : CreateEnrollmentRequest;
