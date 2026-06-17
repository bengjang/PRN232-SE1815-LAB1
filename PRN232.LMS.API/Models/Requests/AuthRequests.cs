using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Models.Requests;

public class LoginRequest
{
    [Required, StringLength(50)]
    public string Username { get; set; } = null!;

    [Required, StringLength(100)]
    public string Password { get; set; } = null!;
}

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}
