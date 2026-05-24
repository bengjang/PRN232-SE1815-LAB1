using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<PagedEntityResult<Course>> GetPagedAsync(QuerySpecification spec, CancellationToken cancellationToken = default);
    Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Course?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<Course> AddAsync(Course entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Course entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Course entity, CancellationToken cancellationToken = default);
}

