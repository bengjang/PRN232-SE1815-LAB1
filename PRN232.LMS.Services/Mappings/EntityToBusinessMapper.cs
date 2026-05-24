using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Mappings;

public static class EntityToBusinessMapper
{
    public static StudentBusinessModel ToBusiness(Student entity, bool includeEnrollments = false) =>
        new()
        {
            StudentId = entity.StudentId,
            FullName = entity.FullName,
            Email = entity.Email,
            DateOfBirth = entity.DateOfBirth,
            Enrollments = includeEnrollments && entity.Enrollments.Count > 0
                ? entity.Enrollments.Select(e => ToBusiness(e, includeStudent: false, includeCourse: true)).ToList()
                : null
        };

    public static SemesterBusinessModel ToBusiness(Semester entity, bool includeCourses = false) =>
        new()
        {
            SemesterId = entity.SemesterId,
            SemesterName = entity.SemesterName,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Courses = includeCourses && entity.Courses.Count > 0
                ? entity.Courses.Select(c => ToBusiness(c, includeSemester: false)).ToList()
                : null
        };

    public static CourseBusinessModel ToBusiness(Course entity, bool includeSemester = false, bool includeEnrollments = false) =>
        new()
        {
            CourseId = entity.CourseId,
            CourseName = entity.CourseName,
            SemesterId = entity.SemesterId,
            Semester = includeSemester && entity.Semester != null
                ? ToBusiness(entity.Semester)
                : null,
            Enrollments = includeEnrollments && entity.Enrollments.Count > 0
                ? entity.Enrollments.Select(e => ToBusiness(e, includeStudent: true, includeCourse: false)).ToList()
                : null
        };

    public static SubjectBusinessModel ToBusiness(Subject entity) =>
        new()
        {
            SubjectId = entity.SubjectId,
            SubjectCode = entity.SubjectCode,
            SubjectName = entity.SubjectName,
            Credit = entity.Credit
        };

    public static EnrollmentBusinessModel ToBusiness(
        Enrollment entity,
        bool includeStudent = false,
        bool includeCourse = false) =>
        new()
        {
            EnrollmentId = entity.EnrollmentId,
            StudentId = entity.StudentId,
            CourseId = entity.CourseId,
            EnrollDate = entity.EnrollDate,
            Status = entity.Status,
            Student = includeStudent && entity.Student != null
                ? ToBusiness(entity.Student)
                : null,
            Course = includeCourse && entity.Course != null
                ? ToBusiness(entity.Course, includeSemester: true)
                : null
        };
}

