using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class EnrollmentRepository : IEnrollmentRepository
{
    private static readonly Dictionary<string, string> SortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["enrollmentid"] = nameof(Enrollment.EnrollmentId),
        ["studentid"] = nameof(Enrollment.StudentId),
        ["courseid"] = nameof(Enrollment.CourseId),
        ["enrolldate"] = nameof(Enrollment.EnrollDate),
        ["status"] = nameof(Enrollment.Status)
    };

    private readonly LmsDbContext _context;

    public EnrollmentRepository(LmsDbContext context) => _context = context;

    public async Task<PagedEntityResult<Enrollment>> GetPagedAsync(QuerySpecification spec, CancellationToken cancellationToken = default)
    {
        spec.Normalize();
        var query = _context.Enrollments.AsNoTracking();

        if (spec.CourseId.HasValue)
            query = query.Where(e => e.CourseId == spec.CourseId.Value);

        if (!string.IsNullOrWhiteSpace(spec.Search))
        {
            var search = spec.Search.Trim().ToLower();
            query = query.Where(e =>
                e.Status.ToLower().Contains(search) ||
                e.Student.FullName.ToLower().Contains(search) ||
                e.Course.CourseName.ToLower().Contains(search));
        }

        query = QuerySortHelper.ApplySort(query, spec.Sort, SortFields);

        if (spec.ShouldExpand("student"))
            query = query.Include(e => e.Student);

        if (spec.ShouldExpand("course"))
            query = query.Include(e => e.Course).ThenInclude(c => c.Semester);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query.Skip((spec.Page - 1) * spec.Size).Take(spec.Size).ToListAsync(cancellationToken);
        return new PagedEntityResult<Enrollment> { Items = items, TotalItems = totalItems };
    }

    public async Task<Enrollment?> GetByIdAsync(int id, QuerySpecification? spec = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Enrollments.AsNoTracking();
        var includeAll = spec is null || spec.Expansions.Count == 0;

        if (includeAll || spec!.ShouldExpand("student"))
            query = query.Include(e => e.Student);

        if (includeAll || spec!.ShouldExpand("course"))
            query = query.Include(e => e.Course).ThenInclude(c => c.Semester);

        return await query.FirstOrDefaultAsync(e => e.EnrollmentId == id, cancellationToken);
    }

    public Task<Enrollment?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Enrollments.AnyAsync(e => e.EnrollmentId == id, cancellationToken);

    public Task<bool> PairExistsAsync(int studentId, int courseId, int? excludeId = null, CancellationToken cancellationToken = default) =>
        _context.Enrollments.AnyAsync(
            e => e.StudentId == studentId &&
                 e.CourseId == courseId &&
                 (!excludeId.HasValue || e.EnrollmentId != excludeId.Value),
            cancellationToken);

    public async Task<Enrollment> AddAsync(Enrollment entity, CancellationToken cancellationToken = default)
    {
        _context.Enrollments.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Enrollment entity, CancellationToken cancellationToken = default)
    {
        _context.Enrollments.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Enrollment entity, CancellationToken cancellationToken = default)
    {
        _context.Enrollments.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

