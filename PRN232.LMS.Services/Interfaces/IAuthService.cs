using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Interfaces;

public interface IAuthService
{
    Task<AuthTokenBusinessModel> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<AuthTokenBusinessModel> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
