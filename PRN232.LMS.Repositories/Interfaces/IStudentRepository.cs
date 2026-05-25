using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IStudentRepository
{
    Task<PagedEntityResult<Student>> GetPagedAsync(QuerySpecification spec, CancellationToken cancellationToken = default);
    Task<Student?> GetByIdAsync(int id, QuerySpecification? spec = null, CancellationToken cancellationToken = default);
    Task<Student?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<Student> AddAsync(Student entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Student entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Student entity, CancellationToken cancellationToken = default);
}

