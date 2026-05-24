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
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService) => _courseService = courseService;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedListResponse<Models.Responses.CourseResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var options = query.ToListQueryOptions();
        var spec = options.ToSpecification();
        var result = await _courseService.GetAllAsync(options, cancellationToken);
        return OkPagedResponse(result, m => BusinessToResponseMapper.ToResponse(m, spec.ShouldExpand("semester"), spec.ShouldExpand("enrollments")), options.GetFields());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CourseResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, [FromQuery] string? fields, CancellationToken cancellationToken)
    {
        var course = await _courseService.GetByIdAsync(id, cancellationToken);
        var fieldList = string.IsNullOrWhiteSpace(fields) ? Array.Empty<string>() : fields.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return OkSingleResponse(BusinessToResponseMapper.ToResponse(course, true, true), fieldList);
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
