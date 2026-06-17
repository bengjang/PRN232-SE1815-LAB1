using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Implementations;

public class AdminService : IAdminService
{
    private readonly IStudentService _studentService;
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;

    public AdminService(
        IStudentService studentService,
        ICourseService courseService,
        IEnrollmentService enrollmentService)
    {
        _studentService = studentService;
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }

    public async Task<AdminDashboardBusinessModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var options = new ListQueryOptions { Page = 1, Size = 1 };
        var students = await _studentService.GetAllAsync(options, cancellationToken);
        var courses = await _courseService.GetAllAsync(options, cancellationToken);
        var enrollments = await _enrollmentService.GetAllAsync(options, cancellationToken);

        return new AdminDashboardBusinessModel
        {
            TotalStudents = students.TotalItems,
            TotalCourses = courses.TotalItems,
            TotalEnrollments = enrollments.TotalItems
        };
    }
}
