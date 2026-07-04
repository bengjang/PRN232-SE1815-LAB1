using PRN232.LMS.Contracts.Exceptions;
using PRN232.LMS.Course.Service.Clients;
using PRN232.LMS.Course.Service.Entities;
using PRN232.LMS.Course.Service.Messaging;
using CourseEntity = PRN232.LMS.Course.Service.Entities.Course;
using PRN232.LMS.Course.Service.Models;
using PRN232.LMS.Course.Service.Repositories;

namespace PRN232.LMS.Course.Service.Services;

public interface ICourseService
{
    Task<List<CourseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CourseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CourseDto> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository) => _courseRepository = courseRepository;

    public async Task<List<CourseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var courses = await _courseRepository.GetAllAsync(cancellationToken);
        return courses.Select(MapToDto).ToList();
    }

    public async Task<CourseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
        if (course is null)
            throw new BusinessException($"Course with id {id} was not found.", 404);
        return MapToDto(course);
    }

    public async Task<CourseDto> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CourseEntity
        {
            CourseName = request.CourseName.Trim(),
            SemesterId = request.SemesterId
        };
        var created = await _courseRepository.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
        if (course is null)
            throw new BusinessException($"Course with id {id} was not found.", 404);
        await _courseRepository.DeleteAsync(course, cancellationToken);
    }

    private static CourseDto MapToDto(CourseEntity course) => new()
    {
        CourseId = course.CourseId,
        CourseName = course.CourseName,
        SemesterId = course.SemesterId,
        SemesterName = course.Semester?.SemesterName
    };
}

public interface IEnrollmentService
{
    Task<List<EnrollmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<EnrollmentDto>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default);
    Task<EnrollmentDto> EnrollStudentAsync(int courseId, EnrollStudentRequest request, CancellationToken cancellationToken = default);
}

public class EnrollmentService : IEnrollmentService
{
    private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Active", "Completed", "Dropped", "Pending"
    };

    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IStudentGrpcClient _studentGrpcClient;
    private readonly IEnrollmentEventPublisher _eventPublisher;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IStudentGrpcClient studentGrpcClient,
        IEnrollmentEventPublisher eventPublisher)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _studentGrpcClient = studentGrpcClient;
        _eventPublisher = eventPublisher;
    }

    public async Task<List<EnrollmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var enrollments = await _enrollmentRepository.GetAllAsync(cancellationToken);
        return await MapEnrollmentsAsync(enrollments, cancellationToken);
    }

    public async Task<List<EnrollmentDto>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default)
    {
        if (!await _courseRepository.ExistsAsync(courseId, cancellationToken))
            throw new BusinessException($"Course with id {courseId} was not found.", 404);

        var enrollments = await _enrollmentRepository.GetByCourseAsync(courseId, cancellationToken);
        return await MapEnrollmentsAsync(enrollments, cancellationToken);
    }

    public async Task<EnrollmentDto> EnrollStudentAsync(int courseId, EnrollStudentRequest request, CancellationToken cancellationToken = default)
    {
        if (request.StudentId <= 0)
            throw new BusinessException("StudentId must be greater than zero.", 400);

        if (string.IsNullOrWhiteSpace(request.Status) || !ValidStatuses.Contains(request.Status.Trim()))
            throw new BusinessException("Status must be one of: Active, Completed, Dropped, Pending.", 400);

        if (!await _courseRepository.ExistsAsync(courseId, cancellationToken))
            throw new BusinessException($"Course with id {courseId} was not found.", 404);

        var studentExists = await _studentGrpcClient.StudentExistsAsync(request.StudentId, cancellationToken);
        if (!studentExists)
            throw new BusinessException($"Student with id {request.StudentId} was not found.", 400);

        if (await _enrollmentRepository.PairExistsAsync(request.StudentId, courseId, cancellationToken: cancellationToken))
            throw new BusinessException("Student is already enrolled in this course.", 400);

        var entity = new Enrollment
        {
            StudentId = request.StudentId,
            CourseId = courseId,
            EnrollDate = DateTime.UtcNow,
            Status = request.Status.Trim()
        };

        var created = await _enrollmentRepository.AddAsync(entity, cancellationToken);
        await _eventPublisher.PublishEnrollmentCompletedAsync(
            new EnrollmentCompletedEvent(
                created.EnrollmentId,
                created.StudentId,
                created.CourseId,
                created.EnrollDate,
                created.Status),
            cancellationToken);

        var studentInfo = await _studentGrpcClient.GetStudentAsync(request.StudentId, cancellationToken);

        return new EnrollmentDto
        {
            EnrollmentId = created.EnrollmentId,
            StudentId = created.StudentId,
            CourseId = created.CourseId,
            EnrollDate = created.EnrollDate,
            Status = created.Status,
            StudentFullName = studentInfo.Found ? studentInfo.FullName : null,
            CourseName = (await _courseRepository.GetByIdAsync(courseId, cancellationToken))?.CourseName
        };
    }

    private async Task<List<EnrollmentDto>> MapEnrollmentsAsync(List<Enrollment> enrollments, CancellationToken cancellationToken)
    {
        var result = new List<EnrollmentDto>();
        foreach (var enrollment in enrollments)
        {
            var studentInfo = await _studentGrpcClient.GetStudentAsync(enrollment.StudentId, cancellationToken);
            result.Add(new EnrollmentDto
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status,
                StudentFullName = studentInfo.Found ? studentInfo.FullName : null,
                CourseName = enrollment.Course?.CourseName
            });
        }
        return result;
    }
}
