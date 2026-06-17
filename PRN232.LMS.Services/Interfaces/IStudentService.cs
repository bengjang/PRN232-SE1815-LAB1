using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Interfaces;

public interface IStudentService
{
    Task<PagedBusinessResult<StudentBusinessModel>> GetAllAsync(ListQueryOptions options, CancellationToken cancellationToken = default);
    Task<PagedBusinessResult<StudentBusinessModel>> GetByCourseAsync(int courseId, ListQueryOptions options, CancellationToken cancellationToken = default);
    Task<StudentBusinessModel> GetByIdAsync(int id, ListQueryOptions? options = null, CancellationToken cancellationToken = default);
    Task<StudentBusinessModel> CreateAsync(string fullName, string email, DateTime dateOfBirth, CancellationToken cancellationToken = default);
    Task<StudentBusinessModel> UpdateAsync(int id, string fullName, string email, DateTime dateOfBirth, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

