using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Interfaces;

public interface IEnrollmentService
{
    Task<PagedBusinessResult<EnrollmentBusinessModel>> GetAllAsync(ListQueryOptions options, CancellationToken cancellationToken = default);
    Task<PagedBusinessResult<EnrollmentBusinessModel>> GetByCourseAsync(int courseId, ListQueryOptions options, CancellationToken cancellationToken = default);
    Task<EnrollmentBusinessModel> GetByIdAsync(int id, ListQueryOptions? options = null, CancellationToken cancellationToken = default);
    Task<EnrollmentBusinessModel> CreateAsync(int studentId, int courseId, DateTime enrollDate, string status, CancellationToken cancellationToken = default);
    Task<EnrollmentBusinessModel> UpdateAsync(int id, int studentId, int courseId, DateTime enrollDate, string status, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

