using PRN232.LMS.Contracts.Grpc;

namespace PRN232.LMS.Course.Service.Clients;

public interface IStudentGrpcClient
{
    Task<StudentResponse> GetStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<bool> StudentExistsAsync(int studentId, CancellationToken cancellationToken = default);
}

public class StudentGrpcClient : IStudentGrpcClient
{
    private readonly StudentGrpc.StudentGrpcClient _client;
    private readonly ILogger<StudentGrpcClient> _logger;

    public StudentGrpcClient(StudentGrpc.StudentGrpcClient client, ILogger<StudentGrpcClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<StudentResponse> GetStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calling Student Service gRPC GetStudent for StudentId={StudentId}", studentId);
        return await _client.GetStudentAsync(new GetStudentRequest { StudentId = studentId }, cancellationToken: cancellationToken);
    }

    public async Task<bool> StudentExistsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calling Student Service gRPC StudentExists for StudentId={StudentId}", studentId);
        var response = await _client.StudentExistsAsync(new StudentExistsRequest { StudentId = studentId }, cancellationToken: cancellationToken);
        return response.Exists;
    }
}
