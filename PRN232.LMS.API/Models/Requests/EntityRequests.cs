using System.ComponentModel.DataAnnotations;
using PRN232.LMS.API.Validation;

namespace PRN232.LMS.API.Models.Requests;

public class CreateStudentRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = null!;

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    [FptStudentCode]
    public string StudentCode { get; set; } = null!;

    [Phone]
    public string? Phone { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }
}

public class UpdateStudentRequest : CreateStudentRequest;

public class CreateSemesterRequest
{
    [Required, StringLength(100)]
    public string SemesterName { get; set; } = null!;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }
}

public class UpdateSemesterRequest : CreateSemesterRequest;

public class CreateCourseRequest
{
    [Required, StringLength(100)]
    public string CourseName { get; set; } = null!;

    [Required, Range(1, int.MaxValue)]
    public int SemesterId { get; set; }
}

public class UpdateCourseRequest : CreateCourseRequest;

public class CreateSubjectRequest
{
    [Required, StringLength(20)]
    public string SubjectCode { get; set; } = null!;

    [Required, StringLength(100)]
    public string SubjectName { get; set; } = null!;

    [Required, Range(1, 20)]
    public int Credit { get; set; }
}

public class UpdateSubjectRequest : CreateSubjectRequest;

public class CreateEnrollmentRequest
{
    [Required, Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    [Required]
    public DateTime EnrollDate { get; set; }

    [Required, StringLength(20)]
    [RegularExpression("^(Active|Completed|Dropped|Pending)$", ErrorMessage = "Status must be Active, Completed, Dropped, or Pending.")]
    public string Status { get; set; } = null!;
}

public class UpdateEnrollmentRequest : CreateEnrollmentRequest;
