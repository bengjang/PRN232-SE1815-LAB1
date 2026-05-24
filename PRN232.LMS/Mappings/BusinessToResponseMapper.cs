using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.API.Mappings;

public static class BusinessToResponseMapper
{
    public static StudentResponse ToResponse(StudentBusinessModel model) =>
        new()
        {
            StudentId = model.StudentId,
            FullName = model.FullName,
            Email = model.Email,
            DateOfBirth = model.DateOfBirth,
            Enrollments = model.Enrollments?.Select(e => ToResponse(e, includeStudent: false, includeCourse: true)).ToList()
        };

    public static SemesterResponse ToResponse(SemesterBusinessModel model) =>
        new()
        {
            SemesterId = model.SemesterId,
            SemesterName = model.SemesterName,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Courses = model.Courses?.Select(c => ToResponse(c, includeSemester: false)).ToList()
        };

    public static CourseResponse ToResponse(CourseBusinessModel model, bool includeSemester = true, bool includeEnrollments = false) =>
        new()
        {
            CourseId = model.CourseId,
            CourseName = model.CourseName,
            SemesterId = model.SemesterId,
            Semester = includeSemester && model.Semester != null ? ToResponse(model.Semester) : null,
            Enrollments = includeEnrollments && model.Enrollments != null
                ? model.Enrollments.Select(e => ToResponse(e, includeStudent: true, includeCourse: false)).ToList()
                : null
        };

    public static SubjectResponse ToResponse(SubjectBusinessModel model) =>
        new()
        {
            SubjectId = model.SubjectId,
            SubjectCode = model.SubjectCode,
            SubjectName = model.SubjectName,
            Credit = model.Credit
        };

    public static EnrollmentResponse ToResponse(EnrollmentBusinessModel model, bool includeStudent = true, bool includeCourse = true) =>
        new()
        {
            EnrollmentId = model.EnrollmentId,
            StudentId = model.StudentId,
            CourseId = model.CourseId,
            EnrollDate = model.EnrollDate,
            Status = model.Status,
            Student = includeStudent && model.Student != null ? ToResponse(model.Student) : null,
            Course = includeCourse && model.Course != null ? ToResponse(model.Course, includeSemester: true) : null
        };
}
