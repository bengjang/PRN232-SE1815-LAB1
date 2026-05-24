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
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService) => _studentService = studentService;

    [HttpGet]
    [SwaggerOperation(Summary = "List students")]
    [ProducesResponseType(typeof(ApiResponse<PagedListResponse<Models.Responses.StudentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var options = query.ToListQueryOptions();
        var result = await _studentService.GetAllAsync(options, cancellationToken);
        return OkPagedResponse(result, BusinessToResponseMapper.ToResponse, options.GetFields());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, [FromQuery] string? fields, CancellationToken cancellationToken)
    {
        var student = await _studentService.GetByIdAsync(id, cancellationToken);
        var fieldList = string.IsNullOrWhiteSpace(fields)
            ? Array.Empty<string>()
            : fields.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return OkSingleResponse(BusinessToResponseMapper.ToResponse(student), fieldList);
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
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequestResponse("Validation failed.", ModelState);
        var updated = await _studentService.UpdateAsync(id, request.FullName, request.Email, request.DateOfBirth, cancellationToken);
        return OkResponse(BusinessToResponseMapper.ToResponse(updated));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _studentService.DeleteAsync(id, cancellationToken);
        return OkResponse<object?>(null, "Student deleted successfully");
    }
}
