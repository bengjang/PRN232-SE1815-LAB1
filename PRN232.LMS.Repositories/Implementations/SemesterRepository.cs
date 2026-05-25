using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class SemesterRepository : ISemesterRepository
{
    private static readonly Dictionary<string, string> SortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["semesterid"] = nameof(Semester.SemesterId),
        ["semestername"] = nameof(Semester.SemesterName),
        ["startdate"] = nameof(Semester.StartDate),
        ["enddate"] = nameof(Semester.EndDate)
    };

    private readonly LmsDbContext _context;

    public SemesterRepository(LmsDbContext context) => _context = context;

    public async Task<PagedEntityResult<Semester>> GetPagedAsync(QuerySpecification spec, CancellationToken cancellationToken = default)
    {
        spec.Normalize();
        var query = _context.Semesters.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(spec.Search))
        {
            var search = spec.Search.Trim().ToLower();
            query = query.Where(s => s.SemesterName.ToLower().Contains(search));
        }

        query = QuerySortHelper.ApplySort(query, spec.Sort, SortFields);

        if (spec.ShouldExpand("courses"))
            query = query.Include(s => s.Courses);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query.Skip((spec.Page - 1) * spec.Size).Take(spec.Size).ToListAsync(cancellationToken);
        return new PagedEntityResult<Semester> { Items = items, TotalItems = totalItems };
    }

    public async Task<Semester?> GetByIdAsync(int id, QuerySpecification? spec = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Semesters.AsNoTracking();
        if (spec is null || spec.Expansions.Count == 0 || spec.ShouldExpand("courses"))
            query = query.Include(s => s.Courses);

        return await query.FirstOrDefaultAsync(s => s.SemesterId == id, cancellationToken);
    }

    public Task<Semester?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Semesters.FirstOrDefaultAsync(s => s.SemesterId == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Semesters.AnyAsync(s => s.SemesterId == id, cancellationToken);

    public async Task<Semester> AddAsync(Semester entity, CancellationToken cancellationToken = default)
    {
        _context.Semesters.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Semester entity, CancellationToken cancellationToken = default)
    {
        _context.Semesters.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Semester entity, CancellationToken cancellationToken = default)
    {
        _context.Semesters.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

