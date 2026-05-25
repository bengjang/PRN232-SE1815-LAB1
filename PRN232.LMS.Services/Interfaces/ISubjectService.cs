using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Interfaces;

public interface ISubjectService
{
    Task<PagedBusinessResult<SubjectBusinessModel>> GetAllAsync(ListQueryOptions options, CancellationToken cancellationToken = default);
    Task<SubjectBusinessModel> GetByIdAsync(int id, ListQueryOptions? options = null, CancellationToken cancellationToken = default);
    Task<SubjectBusinessModel> CreateAsync(string subjectCode, string subjectName, int credit, CancellationToken cancellationToken = default);
    Task<SubjectBusinessModel> UpdateAsync(int id, string subjectCode, string subjectName, int credit, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

