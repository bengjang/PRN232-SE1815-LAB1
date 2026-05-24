using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISubjectRepository
{
    Task<PagedEntityResult<Subject>> GetPagedAsync(QuerySpecification spec, CancellationToken cancellationToken = default);
    Task<Subject?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Subject?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<Subject> AddAsync(Subject entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Subject entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Subject entity, CancellationToken cancellationToken = default);
}

