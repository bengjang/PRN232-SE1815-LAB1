namespace PRN232.LMS.Services.BusinessModels;

public class AuthTokenBusinessModel
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int ExpiresIn { get; set; }
}
