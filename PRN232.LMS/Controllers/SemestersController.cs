using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers;

[Route("api/semesters")]
public class SemestersController : ApiControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService) => _semesterService = semesterService;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedListResponse<Models.Responses.SemesterResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var options = query.ToListQueryOptions();
        var result = await _semesterService.GetAllAsync(options, cancellationToken);
        return OkPagedResponse(result, BusinessToResponseMapper.ToResponse, options.GetFields());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, [FromQuery] string? fields, CancellationToken cancellationToken)
    {
        var semester = await _semesterService.GetByIdAsync(id, cancellationToken);
        var fieldList = string.IsNullOrWhiteSpace(fields) ? Array.Empty<string>() : fields.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return OkSingleResponse(BusinessToResponseMapper.ToResponse(semester), fieldList);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.SemesterResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var created = await _semesterService.CreateAsync(request.SemesterName, request.StartDate, request.EndDate, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = created.SemesterId }, BusinessToResponseMapper.ToResponse(created));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.SemesterResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSemesterRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var updated = await _semesterService.UpdateAsync(id, request.SemesterName, request.StartDate, request.EndDate, cancellationToken);
        return OkResponse(BusinessToResponseMapper.ToResponse(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _semesterService.DeleteAsync(id, cancellationToken);
        return OkResponse<object?>(null, "Semester deleted successfully");
    }
}
