using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class SubjectRepository : ISubjectRepository
{
    private static readonly Dictionary<string, string> SortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["subjectid"] = nameof(Subject.SubjectId),
        ["subjectcode"] = nameof(Subject.SubjectCode),
        ["subjectname"] = nameof(Subject.SubjectName),
        ["credit"] = nameof(Subject.Credit)
    };

    private readonly LmsDbContext _context;

    public SubjectRepository(LmsDbContext context) => _context = context;

    public async Task<PagedEntityResult<Subject>> GetPagedAsync(QuerySpecification spec, CancellationToken cancellationToken = default)
    {
        spec.Normalize();
        var query = _context.Subjects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(spec.Search))
        {
            var search = spec.Search.Trim().ToLower();
            query = query.Where(s =>
                s.SubjectCode.ToLower().Contains(search) ||
                s.SubjectName.ToLower().Contains(search));
        }

        query = QuerySortHelper.ApplySort(query, spec.Sort, SortFields);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query.Skip((spec.Page - 1) * spec.Size).Take(spec.Size).ToListAsync(cancellationToken);
        return new PagedEntityResult<Subject> { Items = items, TotalItems = totalItems };
    }

    public Task<Subject?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.SubjectId == id, cancellationToken);

    public Task<Subject?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Subjects.AnyAsync(s => s.SubjectId == id, cancellationToken);

    public Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim().ToUpper();
        return _context.Subjects.AnyAsync(
            s => s.SubjectCode.ToUpper() == normalized && (!excludeId.HasValue || s.SubjectId != excludeId.Value),
            cancellationToken);
    }

    public async Task<Subject> AddAsync(Subject entity, CancellationToken cancellationToken = default)
    {
        _context.Subjects.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Subject entity, CancellationToken cancellationToken = default)
    {
        _context.Subjects.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Subject entity, CancellationToken cancellationToken = default)
    {
        _context.Subjects.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

