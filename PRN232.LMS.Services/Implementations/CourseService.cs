using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;

namespace PRN232.LMS.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ISemesterRepository _semesterRepository;

    public CourseService(ICourseRepository courseRepository, ISemesterRepository semesterRepository)
    {
        _courseRepository = courseRepository;
        _semesterRepository = semesterRepository;
    }

    public async Task<PagedBusinessResult<CourseBusinessModel>> GetAllAsync(ListQueryOptions options, CancellationToken cancellationToken = default)
    {
        var spec = options.ToSpecification();
        var result = await _courseRepository.GetPagedAsync(spec, cancellationToken);

        return new PagedBusinessResult<CourseBusinessModel>
        {
            Items = result.Items.Select(c => EntityToBusinessMapper.ToBusiness(
                c,
                includeSemester: spec.ShouldExpand("semester"),
                includeEnrollments: spec.ShouldExpand("enrollments"))).ToList(),
            Page = spec.Page,
            PageSize = spec.Size,
            TotalItems = result.TotalItems
        };
    }

    public async Task<CourseBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _courseRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Course with id {id} was not found.", 404);

        return EntityToBusinessMapper.ToBusiness(entity, includeSemester: true, includeEnrollments: true);
    }

    public async Task<CourseBusinessModel> CreateAsync(string courseName, int semesterId, CancellationToken cancellationToken = default)
    {
        if (!await _semesterRepository.ExistsAsync(semesterId, cancellationToken))
            throw new BusinessException($"Semester with id {semesterId} was not found.", 400);

        var entity = new Course
        {
            CourseName = courseName.Trim(),
            SemesterId = semesterId
        };

        var created = await _courseRepository.AddAsync(entity, cancellationToken);
        return EntityToBusinessMapper.ToBusiness(created);
    }

    public async Task<CourseBusinessModel> UpdateAsync(int id, string courseName, int semesterId, CancellationToken cancellationToken = default)
    {
        var entity = await _courseRepository.FindByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Course with id {id} was not found.", 404);

        if (!await _semesterRepository.ExistsAsync(semesterId, cancellationToken))
            throw new BusinessException($"Semester with id {semesterId} was not found.", 400);

        entity.CourseName = courseName.Trim();
        entity.SemesterId = semesterId;

        await _courseRepository.UpdateAsync(entity, cancellationToken);
        return EntityToBusinessMapper.ToBusiness(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _courseRepository.FindByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Course with id {id} was not found.", 404);

        await _courseRepository.DeleteAsync(entity, cancellationToken);
    }
}

