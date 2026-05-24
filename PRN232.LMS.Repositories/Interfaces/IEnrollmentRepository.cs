using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<PagedEntityResult<Enrollment>> GetPagedAsync(QuerySpecification spec, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Enrollment?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> PairExistsAsync(int studentId, int courseId, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<Enrollment> AddAsync(Enrollment entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Enrollment entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Enrollment entity, CancellationToken cancellationToken = default);
}

