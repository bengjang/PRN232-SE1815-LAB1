using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Contracts.Common;
using PRN232.LMS.Student.Service.Models;
using PRN232.LMS.Student.Service.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.Student.Service.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService) => _studentService = studentService;

    [HttpGet]
    [SwaggerOperation(Summary = "List all students")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var students = await _studentService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<List<StudentDto>>.Ok(students));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get student by id")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var student = await _studentService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<StudentDto>.Ok(student));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new student")]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var created = await _studentService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<StudentDto>.Ok(created, "Student created successfully"));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update student")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        var updated = await _studentService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<StudentDto>.Ok(updated));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Delete student (Admin only)")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        await _studentService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.Ok(null, "Student deleted successfully"));
    }
}
