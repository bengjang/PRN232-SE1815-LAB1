using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class StudentRepository : IStudentRepository
{
    private static readonly Dictionary<string, string> SortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["studentid"] = nameof(Student.StudentId),
        ["fullname"] = nameof(Student.FullName),
        ["email"] = nameof(Student.Email),
        ["dateofbirth"] = nameof(Student.DateOfBirth)
    };

    private readonly LmsDbContext _context;

    public StudentRepository(LmsDbContext context) => _context = context;

    public async Task<PagedEntityResult<Student>> GetPagedAsync(QuerySpecification spec, CancellationToken cancellationToken = default)
    {
        spec.Normalize();
        var query = _context.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(spec.Search))
        {
            var search = spec.Search.Trim().ToLower();
            query = query.Where(s =>
                s.FullName.ToLower().Contains(search) ||
                s.Email.ToLower().Contains(search));
        }

        query = QuerySortHelper.ApplySort(query, spec.Sort, SortFields);

        if (spec.ShouldExpand("enrollments"))
            query = query.Include(s => s.Enrollments).ThenInclude(e => e.Course);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((spec.Page - 1) * spec.Size)
            .Take(spec.Size)
            .ToListAsync(cancellationToken);

        return new PagedEntityResult<Student> { Items = items, TotalItems = totalItems };
    }

    public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _context.Students
            .AsNoTracking()
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(s => s.StudentId == id, cancellationToken);

    public Task<Student?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Students.FirstOrDefaultAsync(s => s.StudentId == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Students.AnyAsync(s => s.StudentId == id, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLower();
        return _context.Students.AnyAsync(
            s => s.Email.ToLower() == normalized && (!excludeId.HasValue || s.StudentId != excludeId.Value),
            cancellationToken);
    }

    public async Task<Student> AddAsync(Student entity, CancellationToken cancellationToken = default)
    {
        _context.Students.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Student entity, CancellationToken cancellationToken = default)
    {
        _context.Students.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Student entity, CancellationToken cancellationToken = default)
    {
        _context.Students.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

