using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.API.Mappings;

public static class BusinessToResponseMapper
{
    public static StudentResponse ToResponse(StudentBusinessModel model, bool includeEnrollments = false) =>
        new()
        {
            StudentId = model.StudentId,
            FullName = model.FullName,
            Email = model.Email,
            DateOfBirth = model.DateOfBirth,
            Enrollments = includeEnrollments && model.Enrollments != null
                ? model.Enrollments.Select(e => ToResponse(e, includeStudent: false, includeCourse: true)).ToList()
                : null
        };

    public static StudentResponse ToResponse(StudentBusinessModel model, QuerySpecification spec, bool forDetail) =>
        ToResponse(model, forDetail
            ? spec.ShouldExpandForDetail("enrollments")
            : spec.ShouldExpandForList("enrollments"));

    public static SemesterResponse ToResponse(SemesterBusinessModel model, bool includeCourses = false) =>
        new()
        {
            SemesterId = model.SemesterId,
            SemesterName = model.SemesterName,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Courses = includeCourses && model.Courses != null
                ? model.Courses.Select(c => ToResponse(c, includeSemester: false)).ToList()
                : null
        };

    public static SemesterResponse ToResponse(SemesterBusinessModel model, QuerySpecification spec, bool forDetail) =>
        ToResponse(model, forDetail
            ? spec.ShouldExpandForDetail("courses")
            : spec.ShouldExpandForList("courses"));

    public static CourseResponse ToResponse(CourseBusinessModel model, bool includeSemester = false, bool includeEnrollments = false) =>
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

    public static CourseResponse ToResponse(CourseBusinessModel model, QuerySpecification spec, bool forDetail) =>
        ToResponse(
            model,
            forDetail ? spec.ShouldExpandForDetail("semester") : spec.ShouldExpandForList("semester"),
            forDetail ? spec.ShouldExpandForDetail("enrollments") : spec.ShouldExpandForList("enrollments"));

    public static SubjectResponse ToResponse(SubjectBusinessModel model) =>
        new()
        {
            SubjectId = model.SubjectId,
            SubjectCode = model.SubjectCode,
            SubjectName = model.SubjectName,
            Credit = model.Credit
        };

    public static EnrollmentResponse ToResponse(EnrollmentBusinessModel model, bool includeStudent = false, bool includeCourse = false) =>
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

    public static EnrollmentResponse ToResponse(EnrollmentBusinessModel model, QuerySpecification spec, bool forDetail) =>
        ToResponse(
            model,
            forDetail ? spec.ShouldExpandForDetail("student") : spec.ShouldExpandForList("student"),
            forDetail ? spec.ShouldExpandForDetail("course") : spec.ShouldExpandForList("course"));
}
