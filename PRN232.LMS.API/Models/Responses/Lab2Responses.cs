namespace PRN232.LMS.API.Models.Responses;

public class AuthTokenResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int ExpiresIn { get; set; }
}

public class AdminDashboardResponse
{
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
    public int TotalEnrollments { get; set; }
    public string Message { get; set; } = null!;
}

public class StudentV2Response
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string ApiVersion { get; set; } = "2.0";
}
