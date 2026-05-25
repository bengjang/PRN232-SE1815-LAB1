using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers;

[Route("api/students")]
public class StudentsController : ApiControllerBase
{
    private const string QueryHelp = "search, sort (e.g. fullName,-dateOfBirth), page, size, fields, expand (enrollments)";

    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService) => _studentService = studentService;

    [HttpGet]
    [SwaggerOperation(Summary = "List students", Description = QueryHelp)]
    [ProducesResponseType(typeof(ApiResponse<PagedListResponse<Models.Responses.StudentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var result = await _studentService.GetAllAsync(ToOptions(query), cancellationToken);
        return OkPagedResponse(result, m => BusinessToResponseMapper.ToResponse(m, spec, forDetail: false), query);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get student by id", Description = QueryHelp + ". Omit expand = include all related data.")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, [FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var spec = ToSpec(query);
        var student = await _studentService.GetByIdAsync(id, ToOptions(query), cancellationToken);
        return OkDetailResponse(BusinessToResponseMapper.ToResponse(student, spec, forDetail: true), query);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var created = await _studentService.CreateAsync(request.FullName, request.Email, request.DateOfBirth, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = created.StudentId }, BusinessToResponseMapper.ToResponse(created));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.StudentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var updated = await _studentService.UpdateAsync(id, request.FullName, request.Email, request.DateOfBirth, cancellationToken);
        return OkResponse(BusinessToResponseMapper.ToResponse(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _studentService.DeleteAsync(id, cancellationToken);
        return OkResponse<object?>(null, "Student deleted successfully");
    }
}
