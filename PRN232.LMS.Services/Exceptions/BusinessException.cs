namespace PRN232.LMS.Services.Exceptions;

public class BusinessException : Exception
{
    public int StatusCode { get; }
    public object? Errors { get; }

    public BusinessException(string message, int statusCode = 400, object? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

