using Grpc.Core;
using PRN232.LMS.Contracts.Grpc;
using PRN232.LMS.Student.Service.Services;

namespace PRN232.LMS.Student.Service.GrpcServices;

public class StudentGrpcService : StudentGrpc.StudentGrpcBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentGrpcService> _logger;

    public StudentGrpcService(IStudentService studentService, ILogger<StudentGrpcService> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    public override async Task<StudentResponse> GetStudent(GetStudentRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC GetStudent called for StudentId={StudentId}", request.StudentId);

        try
        {
            var student = await _studentService.GetByIdAsync(request.StudentId, context.CancellationToken);
            return new StudentResponse
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth.ToString("yyyy-MM-dd"),
                Found = true
            };
        }
        catch (PRN232.LMS.Contracts.Exceptions.BusinessException)
        {
            return new StudentResponse { Found = false };
        }
    }

    public override async Task<StudentExistsResponse> StudentExists(StudentExistsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC StudentExists called for StudentId={StudentId}", request.StudentId);
        var exists = await _studentService.ExistsAsync(request.StudentId, context.CancellationToken);
        return new StudentExistsResponse { Exists = exists };
    }
}
