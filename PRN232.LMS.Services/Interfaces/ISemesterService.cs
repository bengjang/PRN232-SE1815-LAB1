using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Interfaces;

public interface ISemesterService
{
    Task<PagedBusinessResult<SemesterBusinessModel>> GetAllAsync(ListQueryOptions options, CancellationToken cancellationToken = default);
    Task<SemesterBusinessModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SemesterBusinessModel> CreateAsync(string semesterName, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<SemesterBusinessModel> UpdateAsync(int id, string semesterName, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

