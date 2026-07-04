using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Course.Service.Entities;

namespace PRN232.LMS.Course.Service.Repositories;

public interface IEnrollmentRepository
{
    Task<List<Enrollment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Enrollment>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> PairExistsAsync(int studentId, int courseId, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<Enrollment> AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
}

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly Data.CourseDbContext _context;

    public EnrollmentRepository(Data.CourseDbContext context) => _context = context;

    public Task<List<Enrollment>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Enrollments.Include(e => e.Course).OrderBy(e => e.EnrollmentId).ToListAsync(cancellationToken);

    public Task<List<Enrollment>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default) =>
        _context.Enrollments.Where(e => e.CourseId == courseId).OrderBy(e => e.EnrollmentId).ToListAsync(cancellationToken);

    public Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Enrollments.Include(e => e.Course).FirstOrDefaultAsync(e => e.EnrollmentId == id, cancellationToken);

    public Task<bool> PairExistsAsync(int studentId, int courseId, int? excludeId = null, CancellationToken cancellationToken = default) =>
        _context.Enrollments.AnyAsync(
            e => e.StudentId == studentId && e.CourseId == courseId && (!excludeId.HasValue || e.EnrollmentId != excludeId),
            cancellationToken);

    public async Task<Enrollment> AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return enrollment;
    }

    public async Task DeleteAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
