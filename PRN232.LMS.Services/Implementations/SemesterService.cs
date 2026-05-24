using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;

namespace PRN232.LMS.Services.Implementations;

public class SemesterService : ISemesterService
{
    private readonly ISemesterRepository _repository;

    public SemesterService(ISemesterRepository repository) => _repository = repository;

    public async Task<PagedBusinessResult<SemesterBusinessModel>> GetAllAsync(ListQueryOptions options, CancellationToken cancellationToken = default)
    {
        var spec = options.ToSpecification();
        var result = await _repository.GetPagedAsync(spec, cancellationToken);

        return new PagedBusinessResult<SemesterBusinessModel>
        {
            Items = result.Items.Select(s => EntityToBusinessMapper.ToBusiness(s, spec.ShouldExpand("courses"))).ToList(),
            Page = spec.Page,
            PageSize = spec.Size,
            TotalItems = result.TotalItems
        };
    }

    public async Task<SemesterBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Semester with id {id} was not found.", 404);

        return EntityToBusinessMapper.ToBusiness(entity, includeCourses: true);
    }

    public async Task<SemesterBusinessModel> CreateAsync(string semesterName, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        if (endDate <= startDate)
            throw new BusinessException("EndDate must be after StartDate.", 400);

        var entity = new Semester
        {
            SemesterName = semesterName.Trim(),
            StartDate = startDate,
            EndDate = endDate
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return EntityToBusinessMapper.ToBusiness(created);
    }

    public async Task<SemesterBusinessModel> UpdateAsync(int id, string semesterName, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.FindByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Semester with id {id} was not found.", 404);

        if (endDate <= startDate)
            throw new BusinessException("EndDate must be after StartDate.", 400);

        entity.SemesterName = semesterName.Trim();
        entity.StartDate = startDate;
        entity.EndDate = endDate;

        await _repository.UpdateAsync(entity, cancellationToken);
        return EntityToBusinessMapper.ToBusiness(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.FindByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Semester with id {id} was not found.", 404);

        await _repository.DeleteAsync(entity, cancellationToken);
    }
}

