using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers;

[Route("api/courses")]
public class CoursesController : ApiControllerBase
{
    private const string QueryHelp = "search, sort, page, size, fields, expand (semester,enrollments)";

    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;

    public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List courses", Description = QueryHelp)]
    [ProducesResponseType(typeof(ApiResponse<PagedListResponse<Models.Responses.CourseResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var result = await _courseService.GetAllAsync(ToOptions(query), cancellationToken);
        return OkPagedResponse(result, m => BusinessToResponseMapper.ToResponse(m, spec, forDetail: false), query);
    }

    [HttpGet("{id:int}/enrollments")]
    [SwaggerOperation(Summary = "List enrollments of a course", Description = QueryHelp + " + expand student,course")]
    [ProducesResponseType(typeof(ApiResponse<PagedListResponse<Models.Responses.EnrollmentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollments(int id, [FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var result = await _enrollmentService.GetByCourseAsync(id, ToOptions(query), cancellationToken);
        return OkPagedResponse(result, m => BusinessToResponseMapper.ToResponse(m, spec, forDetail: false), query);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get course by id", Description = QueryHelp)]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, [FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var course = await _courseService.GetByIdAsync(id, ToOptions(query), cancellationToken);
        return OkDetailResponse(BusinessToResponseMapper.ToResponse(course, spec, forDetail: true), query);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CourseResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var created = await _courseService.CreateAsync(request.CourseName, request.SemesterId, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = created.CourseId }, BusinessToResponseMapper.ToResponse(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var updated = await _courseService.UpdateAsync(id, request.CourseName, request.SemesterId, cancellationToken);
        return OkResponse(BusinessToResponseMapper.ToResponse(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _courseService.DeleteAsync(id, cancellationToken);
        return OkResponse<object?>(null, "Course deleted successfully");
    }
}
