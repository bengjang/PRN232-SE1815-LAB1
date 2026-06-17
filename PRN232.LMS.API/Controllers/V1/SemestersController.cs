using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/semesters")]
[Authorize]
public class SemestersController : ApiControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService) => _semesterService = semesterService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var result = await _semesterService.GetAllAsync(ToOptions(query), cancellationToken);
        return OkPagedResponse(result, m => BusinessToResponseMapper.ToResponse(m, spec, forDetail: false), query);
    }

    [HttpGet("{id:int}", Name = "GetSemesterById")]
    public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var semester = await _semesterService.GetByIdAsync(id, ToOptions(query), cancellationToken);
        return OkDetailResponse(BusinessToResponseMapper.ToResponse(semester, spec, forDetail: true), query);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var created = await _semesterService.CreateAsync(request.SemesterName, request.StartDate, request.EndDate, cancellationToken);
        return CreatedResponse("GetSemesterById", new { id = created.SemesterId, version = "1.0" }, BusinessToResponseMapper.ToResponse(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateSemesterRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var updated = await _semesterService.UpdateAsync(id, request.SemesterName, request.StartDate, request.EndDate, cancellationToken);
        return OkResponse(BusinessToResponseMapper.ToResponse(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        await _semesterService.DeleteAsync(id, cancellationToken);
        return OkResponse<object?>(null, "Semester deleted successfully");
    }
}
