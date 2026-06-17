using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ApiControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService) => _adminService = adminService;

    [HttpGet("dashboard")]
    [SwaggerOperation(Summary = "Admin dashboard (Admin only)")]
    [ProducesResponseType(typeof(ApiResponse<AdminDashboardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var data = await _adminService.GetDashboardAsync(cancellationToken);
        return OkResponse(new AdminDashboardResponse
        {
            TotalStudents = data.TotalStudents,
            TotalCourses = data.TotalCourses,
            TotalEnrollments = data.TotalEnrollments,
            Message = "Admin dashboard loaded successfully"
        });
    }
}
