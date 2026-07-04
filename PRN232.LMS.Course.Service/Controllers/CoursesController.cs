using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Contracts.Common;
using PRN232.LMS.Course.Service.Models;
using PRN232.LMS.Course.Service.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.Course.Service.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;

    public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List all courses")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var courses = await _courseService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<List<CourseDto>>.Ok(courses));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get course by id")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var course = await _courseService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<CourseDto>.Ok(course));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    [SwaggerOperation(Summary = "Create a new course")]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        var created = await _courseService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<CourseDto>.Ok(created, "Course created successfully"));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Delete course (Admin only)")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        await _courseService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.Ok(null, "Course deleted successfully"));
    }

    [HttpGet("{courseId:int}/enrollments")]
    [SwaggerOperation(Summary = "List enrollments for a course")]
    public async Task<IActionResult> GetEnrollments([FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentService.GetByCourseAsync(courseId, cancellationToken);
        return Ok(ApiResponse<List<EnrollmentDto>>.Ok(enrollments));
    }

    [HttpPost("{courseId:int}/enrollments")]
    [SwaggerOperation(Summary = "Enroll a student into a course (verifies student via gRPC)")]
    public async Task<IActionResult> EnrollStudent(
        [FromRoute] int courseId,
        [FromBody] EnrollStudentRequest request,
        CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentService.EnrollStudentAsync(courseId, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<EnrollmentDto>.Ok(enrollment, "Enrollment completed successfully"));
    }
}

[ApiController]
[Route("api/enrollments")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService) => _enrollmentService = enrollmentService;

    [HttpGet]
    [SwaggerOperation(Summary = "List all enrollments")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<List<EnrollmentDto>>.Ok(enrollments));
    }
}
