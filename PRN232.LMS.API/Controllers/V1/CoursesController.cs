using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/courses")]
[Authorize]
public class CoursesController : ApiControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly IStudentService _studentService;

    public CoursesController(
        ICourseService courseService,
        IEnrollmentService enrollmentService,
        IStudentService studentService)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var result = await _courseService.GetAllAsync(ToOptions(query), cancellationToken);
        return OkPagedResponse(result, m => BusinessToResponseMapper.ToResponse(m, spec, forDetail: false), query);
    }

    [HttpGet("{courseId:int}/students", Name = "GetStudentsByCourse")]
    [SwaggerOperation(Summary = "Nested resource: students enrolled in a course")]
    public async Task<IActionResult> GetStudentsByCourse(
        [FromRoute] int courseId,
        [FromQuery] CollectionQueryRequest query,
        CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var result = await _studentService.GetByCourseAsync(courseId, ToOptions(query), cancellationToken);
        return OkPagedResponse(result, m => BusinessToResponseMapper.ToResponse(m, spec, forDetail: false), query);
    }

    [HttpGet("{courseId:int}/enrollments")]
    public async Task<IActionResult> GetEnrollments(
        [FromRoute] int courseId,
        [FromQuery] CollectionQueryRequest query,
        CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var result = await _enrollmentService.GetByCourseAsync(courseId, ToOptions(query), cancellationToken);
        return OkPagedResponse(result, m => BusinessToResponseMapper.ToResponse(m, spec, forDetail: false), query);
    }

    [HttpGet("{id:int}", Name = "GetCourseById")]
    public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var course = await _courseService.GetByIdAsync(id, ToOptions(query), cancellationToken);
        return OkDetailResponse(BusinessToResponseMapper.ToResponse(course, spec, forDetail: true), query);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var created = await _courseService.CreateAsync(request.CourseName, request.SemesterId, cancellationToken);
        return CreatedResponse("GetCourseById", new { id = created.CourseId, version = "1.0" }, BusinessToResponseMapper.ToResponse(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var updated = await _courseService.UpdateAsync(id, request.CourseName, request.SemesterId, cancellationToken);
        return OkResponse(BusinessToResponseMapper.ToResponse(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        await _courseService.DeleteAsync(id, cancellationToken);
        return OkResponse<object?>(null, "Course deleted successfully");
    }
}
