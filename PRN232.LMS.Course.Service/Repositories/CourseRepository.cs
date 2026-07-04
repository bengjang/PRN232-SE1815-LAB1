using Microsoft.EntityFrameworkCore;
using CourseEntity = PRN232.LMS.Course.Service.Entities.Course;

namespace PRN232.LMS.Course.Service.Repositories;

public interface ICourseRepository
{
    Task<List<CourseEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CourseEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<CourseEntity> AddAsync(CourseEntity course, CancellationToken cancellationToken = default);
    Task UpdateAsync(CourseEntity course, CancellationToken cancellationToken = default);
    Task DeleteAsync(CourseEntity course, CancellationToken cancellationToken = default);
}

public class CourseRepository : ICourseRepository
{
    private readonly Data.CourseDbContext _context;

    public CourseRepository(Data.CourseDbContext context) => _context = context;

    public Task<List<CourseEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Courses.Include(c => c.Semester).OrderBy(c => c.CourseId).ToListAsync(cancellationToken);

    public Task<CourseEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Courses.Include(c => c.Semester).FirstOrDefaultAsync(c => c.CourseId == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Courses.AnyAsync(c => c.CourseId == id, cancellationToken);

    public async Task<CourseEntity> AddAsync(CourseEntity course, CancellationToken cancellationToken = default)
    {
        await _context.Courses.AddAsync(course, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return course;
    }

    public async Task UpdateAsync(CourseEntity course, CancellationToken cancellationToken = default)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(CourseEntity course, CancellationToken cancellationToken = default)
    {
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
