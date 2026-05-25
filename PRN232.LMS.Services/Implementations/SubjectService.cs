using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;

namespace PRN232.LMS.Services.Implementations;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _repository;

    public SubjectService(ISubjectRepository repository) => _repository = repository;

    public async Task<PagedBusinessResult<SubjectBusinessModel>> GetAllAsync(ListQueryOptions options, CancellationToken cancellationToken = default)
    {
        var spec = options.ToSpecification();
        var result = await _repository.GetPagedAsync(spec, cancellationToken);

        return new PagedBusinessResult<SubjectBusinessModel>
        {
            Items = result.Items.Select(EntityToBusinessMapper.ToBusiness).ToList(),
            Page = spec.Page,
            PageSize = spec.Size,
            TotalItems = result.TotalItems
        };
    }

    public async Task<SubjectBusinessModel> GetByIdAsync(int id, ListQueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Subject with id {id} was not found.", 404);

        return EntityToBusinessMapper.ToBusiness(entity);
    }

    public async Task<SubjectBusinessModel> CreateAsync(string subjectCode, string subjectName, int credit, CancellationToken cancellationToken = default)
    {
        if (credit <= 0)
            throw new BusinessException("Credit must be greater than zero.", 400);

        if (await _repository.CodeExistsAsync(subjectCode, cancellationToken: cancellationToken))
            throw new BusinessException("Subject code already exists.", 400);

        var entity = new Subject
        {
            SubjectCode = subjectCode.Trim().ToUpper(),
            SubjectName = subjectName.Trim(),
            Credit = credit
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return EntityToBusinessMapper.ToBusiness(created);
    }

    public async Task<SubjectBusinessModel> UpdateAsync(int id, string subjectCode, string subjectName, int credit, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.FindByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Subject with id {id} was not found.", 404);

        if (credit <= 0)
            throw new BusinessException("Credit must be greater than zero.", 400);

        if (await _repository.CodeExistsAsync(subjectCode, id, cancellationToken))
            throw new BusinessException("Subject code already exists.", 400);

        entity.SubjectCode = subjectCode.Trim().ToUpper();
        entity.SubjectName = subjectName.Trim();
        entity.Credit = credit;

        await _repository.UpdateAsync(entity, cancellationToken);
        return EntityToBusinessMapper.ToBusiness(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.FindByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Subject with id {id} was not found.", 404);

        await _repository.DeleteAsync(entity, cancellationToken);
    }
}

