using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Contracts.Common;
using PRN232.LMS.Identity.Service.Models;
using PRN232.LMS.Identity.Service.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.Identity.Service.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login and receive JWT tokens")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Username, request.Password, cancellationToken);
        return Ok(ApiResponse<AuthTokenDto>.Ok(result));
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(Summary = "Refresh access token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        return Ok(ApiResponse<AuthTokenDto>.Ok(result));
    }
}
