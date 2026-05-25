using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISemesterRepository
{
    Task<PagedEntityResult<Semester>> GetPagedAsync(QuerySpecification spec, CancellationToken cancellationToken = default);
    Task<Semester?> GetByIdAsync(int id, QuerySpecification? spec = null, CancellationToken cancellationToken = default);
    Task<Semester?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<Semester> AddAsync(Semester entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Semester entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Semester entity, CancellationToken cancellationToken = default);
}

