using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers;

[Route("api/enrollments")]
public class EnrollmentsController : ApiControllerBase
{
    private const string QueryHelp = "search=active, sort=-enrollDate, page, size, fields, expand (student,course)";

    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService) => _enrollmentService = enrollmentService;

    [HttpGet]
    [SwaggerOperation(Summary = "List enrollments", Description = QueryHelp)]
    [ProducesResponseType(typeof(ApiResponse<PagedListResponse<Models.Responses.EnrollmentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var result = await _enrollmentService.GetAllAsync(ToOptions(query), cancellationToken);
        return OkPagedResponse(result, m => BusinessToResponseMapper.ToResponse(m, spec, forDetail: false), query);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get enrollment by id", Description = QueryHelp)]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, [FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var enrollment = await _enrollmentService.GetByIdAsync(id, ToOptions(query), cancellationToken);
        return OkDetailResponse(BusinessToResponseMapper.ToResponse(enrollment, spec, forDetail: true), query);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var created = await _enrollmentService.CreateAsync(request.StudentId, request.CourseId, request.EnrollDate, request.Status, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = created.EnrollmentId }, BusinessToResponseMapper.ToResponse(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var updated = await _enrollmentService.UpdateAsync(id, request.StudentId, request.CourseId, request.EnrollDate, request.Status, cancellationToken);
        return OkResponse(BusinessToResponseMapper.ToResponse(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _enrollmentService.DeleteAsync(id, cancellationToken);
        return OkResponse<object?>(null, "Enrollment deleted successfully");
    }
}
