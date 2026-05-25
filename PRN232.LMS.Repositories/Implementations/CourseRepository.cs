using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class CourseRepository : ICourseRepository
{
    private static readonly Dictionary<string, string> SortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["courseid"] = nameof(Course.CourseId),
        ["coursename"] = nameof(Course.CourseName),
        ["semesterid"] = nameof(Course.SemesterId)
    };

    private readonly LmsDbContext _context;

    public CourseRepository(LmsDbContext context) => _context = context;

    public async Task<PagedEntityResult<Course>> GetPagedAsync(QuerySpecification spec, CancellationToken cancellationToken = default)
    {
        spec.Normalize();
        var query = _context.Courses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(spec.Search))
        {
            var search = spec.Search.Trim().ToLower();
            query = query.Where(c => c.CourseName.ToLower().Contains(search));
        }

        query = QuerySortHelper.ApplySort(query, spec.Sort, SortFields);

        if (spec.ShouldExpand("semester"))
            query = query.Include(c => c.Semester);

        if (spec.ShouldExpand("enrollments"))
            query = query.Include(c => c.Enrollments).ThenInclude(e => e.Student);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query.Skip((spec.Page - 1) * spec.Size).Take(spec.Size).ToListAsync(cancellationToken);
        return new PagedEntityResult<Course> { Items = items, TotalItems = totalItems };
    }

    public async Task<Course?> GetByIdAsync(int id, QuerySpecification? spec = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Courses.AsNoTracking();
        var includeAll = spec is null || spec.Expansions.Count == 0;

        if (includeAll || spec!.ShouldExpand("semester"))
            query = query.Include(c => c.Semester);

        if (includeAll || spec!.ShouldExpand("enrollments"))
            query = query.Include(c => c.Enrollments).ThenInclude(e => e.Student);

        return await query.FirstOrDefaultAsync(c => c.CourseId == id, cancellationToken);
    }

    public Task<Course?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Courses.AnyAsync(c => c.CourseId == id, cancellationToken);

    public async Task<Course> AddAsync(Course entity, CancellationToken cancellationToken = default)
    {
        _context.Courses.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Course entity, CancellationToken cancellationToken = default)
    {
        _context.Courses.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Course entity, CancellationToken cancellationToken = default)
    {
        _context.Courses.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

