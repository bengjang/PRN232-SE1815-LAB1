using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Interfaces;

public interface ICourseService
{
    Task<PagedBusinessResult<CourseBusinessModel>> GetAllAsync(ListQueryOptions options, CancellationToken cancellationToken = default);
    Task<CourseBusinessModel> GetByIdAsync(int id, ListQueryOptions? options = null, CancellationToken cancellationToken = default);
    Task<CourseBusinessModel> CreateAsync(string courseName, int semesterId, CancellationToken cancellationToken = default);
    Task<CourseBusinessModel> UpdateAsync(int id, string courseName, int semesterId, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

