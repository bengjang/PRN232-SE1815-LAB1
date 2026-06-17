using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers.V2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/students")]
[Authorize]
public class StudentsController : ApiControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService) => _studentService = studentService;

    [HttpGet]
    [SwaggerOperation(Summary = "List students (API v2 - simplified response)")]
    public async Task<IActionResult> GetAll([FromQuery] CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var result = await _studentService.GetAllAsync(ToOptions(query), cancellationToken);
        var items = result.Items.Select(s => new StudentV2Response
        {
            StudentId = s.StudentId,
            StudentCode = $"SE{s.StudentId:D5}",
            FullName = s.FullName,
            Email = s.Email
        }).ToList();

        var data = new PagedListResponse<StudentV2Response>
        {
            Items = items,
            Pagination = new PaginationMetadata
            {
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            }
        };

        return Ok(ApiResponse<PagedListResponse<StudentV2Response>>.Ok(data));
    }

    [HttpGet("{id:int}", Name = "GetStudentByIdV2")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var student = await _studentService.GetByIdAsync(id, cancellationToken: cancellationToken);
        return OkResponse(new StudentV2Response
        {
            StudentId = student.StudentId,
            StudentCode = $"SE{student.StudentId:D5}",
            FullName = student.FullName,
            Email = student.Email
        });
    }
}
