using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers;

[Route("api/subjects")]
public class SubjectsController : ApiControllerBase
{
    private const string QueryHelp = "search, sort (subjectCode,subjectName,credit), page, size, fields";

    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService) => _subjectService = subjectService;

    [HttpGet]
    [SwaggerOperation(Summary = "List subjects", Description = QueryHelp)]
    [ProducesResponseType(typeof(ApiResponse<PagedListResponse<Models.Responses.SubjectResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var result = await _subjectService.GetAllAsync(ToOptions(query), cancellationToken);
        return OkPagedResponse(result, BusinessToResponseMapper.ToResponse, query);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get subject by id", Description = QueryHelp)]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, [FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var subject = await _subjectService.GetByIdAsync(id, ToOptions(query), cancellationToken);
        return OkDetailResponse(BusinessToResponseMapper.ToResponse(subject), query);
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
