using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;

namespace PRN232.LMS.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository) => _repository = repository;

    public async Task<PagedBusinessResult<StudentBusinessModel>> GetAllAsync(ListQueryOptions options, CancellationToken cancellationToken = default)
    {
        var spec = options.ToSpecification();
        var result = await _repository.GetPagedAsync(spec, cancellationToken);
        var includeEnrollments = spec.ShouldExpand("enrollments");

        return new PagedBusinessResult<StudentBusinessModel>
        {
            Items = result.Items.Select(s => EntityToBusinessMapper.ToBusiness(s, includeEnrollments)).ToList(),
            Page = spec.Page,
            PageSize = spec.Size,
            TotalItems = result.TotalItems
        };
    }

    public async Task<StudentBusinessModel> GetByIdAsync(int id, ListQueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var spec = options?.ToSpecification();
        var entity = await _repository.GetByIdAsync(id, spec, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Student with id {id} was not found.", 404);

        var includeEnrollments = spec is null || spec.Expansions.Count == 0 || spec.ShouldExpandForDetail("enrollments");
        return EntityToBusinessMapper.ToBusiness(entity, includeEnrollments);
    }

    public async Task<StudentBusinessModel> CreateAsync(string fullName, string email, DateTime dateOfBirth, CancellationToken cancellationToken = default)
    {
        if (await _repository.EmailExistsAsync(email, cancellationToken: cancellationToken))
            throw new BusinessException("Email already exists.", 400);

        var entity = new Student
        {
            FullName = fullName.Trim(),
            Email = email.Trim(),
            DateOfBirth = dateOfBirth
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return EntityToBusinessMapper.ToBusiness(created);
    }

    public async Task<StudentBusinessModel> UpdateAsync(int id, string fullName, string email, DateTime dateOfBirth, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.FindByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Student with id {id} was not found.", 404);

        if (await _repository.EmailExistsAsync(email, id, cancellationToken))
            throw new BusinessException("Email already exists.", 400);

        entity.FullName = fullName.Trim();
        entity.Email = email.Trim();
        entity.DateOfBirth = dateOfBirth;

        await _repository.UpdateAsync(entity, cancellationToken);
        return EntityToBusinessMapper.ToBusiness(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.FindByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Student with id {id} was not found.", 404);

        await _repository.DeleteAsync(entity, cancellationToken);
    }
}

