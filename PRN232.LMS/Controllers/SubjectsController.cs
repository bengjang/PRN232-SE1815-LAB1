using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.API.Controllers;

[Route("api/subjects")]
public class SubjectsController : ApiControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService) => _subjectService = subjectService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var options = query.ToListQueryOptions();
        var result = await _subjectService.GetAllAsync(options, cancellationToken);
        return OkPagedResponse(result, BusinessToResponseMapper.ToResponse, options.GetFields());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] string? fields, CancellationToken cancellationToken)
    {
        var subject = await _subjectService.GetByIdAsync(id, cancellationToken);
        var fieldList = string.IsNullOrWhiteSpace(fields) ? Array.Empty<string>() : fields.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return OkSingleResponse(BusinessToResponseMapper.ToResponse(subject), fieldList);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubjectRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var created = await _subjectService.CreateAsync(request.SubjectCode, request.SubjectName, request.Credit, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = created.SubjectId }, BusinessToResponseMapper.ToResponse(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSubjectRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var updated = await _subjectService.UpdateAsync(id, request.SubjectCode, request.SubjectName, request.Credit, cancellationToken);
        return OkResponse(BusinessToResponseMapper.ToResponse(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _subjectService.DeleteAsync(id, cancellationToken);
        return OkResponse<object?>(null, "Subject deleted successfully");
    }
}
