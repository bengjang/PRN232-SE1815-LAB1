using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;

namespace PRN232.LMS.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Active", "Completed", "Dropped", "Pending"
    };

    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<PagedBusinessResult<EnrollmentBusinessModel>> GetAllAsync(ListQueryOptions options, CancellationToken cancellationToken = default)
    {
        var spec = options.ToSpecification();
        var result = await _enrollmentRepository.GetPagedAsync(spec, cancellationToken);

        return new PagedBusinessResult<EnrollmentBusinessModel>
        {
            Items = result.Items.Select(e => EntityToBusinessMapper.ToBusiness(
                e,
                includeStudent: spec.ShouldExpand("student"),
                includeCourse: spec.ShouldExpand("course"))).ToList(),
            Page = spec.Page,
            PageSize = spec.Size,
            TotalItems = result.TotalItems
        };
    }

    public async Task<EnrollmentBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _enrollmentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Enrollment with id {id} was not found.", 404);

        return EntityToBusinessMapper.ToBusiness(entity, includeStudent: true, includeCourse: true);
    }

    public async Task<EnrollmentBusinessModel> CreateAsync(int studentId, int courseId, DateTime enrollDate, string status, CancellationToken cancellationToken = default)
    {
        ValidateEnrollmentInput(studentId, courseId, status);

        if (!await _studentRepository.ExistsAsync(studentId, cancellationToken))
            throw new BusinessException($"Student with id {studentId} was not found.", 400);

        if (!await _courseRepository.ExistsAsync(courseId, cancellationToken))
            throw new BusinessException($"Course with id {courseId} was not found.", 400);

        if (await _enrollmentRepository.PairExistsAsync(studentId, courseId, cancellationToken: cancellationToken))
            throw new BusinessException("Student is already enrolled in this course.", 400);

        var entity = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrollDate = enrollDate,
            Status = status.Trim()
        };

        var created = await _enrollmentRepository.AddAsync(entity, cancellationToken);
        return EntityToBusinessMapper.ToBusiness(created);
    }

    public async Task<EnrollmentBusinessModel> UpdateAsync(int id, int studentId, int courseId, DateTime enrollDate, string status, CancellationToken cancellationToken = default)
    {
        var entity = await _enrollmentRepository.FindByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Enrollment with id {id} was not found.", 404);

        ValidateEnrollmentInput(studentId, courseId, status);

        if (!await _studentRepository.ExistsAsync(studentId, cancellationToken))
            throw new BusinessException($"Student with id {studentId} was not found.", 400);

        if (!await _courseRepository.ExistsAsync(courseId, cancellationToken))
            throw new BusinessException($"Course with id {courseId} was not found.", 400);

        if (await _enrollmentRepository.PairExistsAsync(studentId, courseId, id, cancellationToken))
            throw new BusinessException("Student is already enrolled in this course.", 400);

        entity.StudentId = studentId;
        entity.CourseId = courseId;
        entity.EnrollDate = enrollDate;
        entity.Status = status.Trim();

        await _enrollmentRepository.UpdateAsync(entity, cancellationToken);
        return EntityToBusinessMapper.ToBusiness(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _enrollmentRepository.FindByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Enrollment with id {id} was not found.", 404);

        await _enrollmentRepository.DeleteAsync(entity, cancellationToken);
    }

    private static void ValidateEnrollmentInput(int studentId, int courseId, string status)
    {
        if (studentId <= 0 || courseId <= 0)
            throw new BusinessException("StudentId and CourseId must be greater than zero.", 400);

        if (string.IsNullOrWhiteSpace(status) || !ValidStatuses.Contains(status.Trim()))
            throw new BusinessException("Status must be one of: Active, Completed, Dropped, Pending.", 400);
    }
}

