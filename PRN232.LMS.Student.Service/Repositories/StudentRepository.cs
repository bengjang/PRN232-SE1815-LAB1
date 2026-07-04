using Microsoft.EntityFrameworkCore;
using StudentEntity = PRN232.LMS.Student.Service.Entities.Student;

namespace PRN232.LMS.Student.Service.Repositories;

public interface IStudentRepository
{
    Task<List<StudentEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StudentEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<StudentEntity> AddAsync(StudentEntity student, CancellationToken cancellationToken = default);
    Task UpdateAsync(StudentEntity student, CancellationToken cancellationToken = default);
    Task DeleteAsync(StudentEntity student, CancellationToken cancellationToken = default);
}

public class StudentRepository : IStudentRepository
{
    private readonly Data.StudentDbContext _context;

    public StudentRepository(Data.StudentDbContext context) => _context = context;

    public Task<List<StudentEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Students.OrderBy(s => s.StudentId).ToListAsync(cancellationToken);

    public Task<StudentEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Students.FirstOrDefaultAsync(s => s.StudentId == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Students.AnyAsync(s => s.StudentId == id, cancellationToken);

    public async Task<StudentEntity> AddAsync(StudentEntity student, CancellationToken cancellationToken = default)
    {
        await _context.Students.AddAsync(student, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return student;
    }

    public async Task UpdateAsync(StudentEntity student, CancellationToken cancellationToken = default)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(StudentEntity student, CancellationToken cancellationToken = default)
    {
        _context.Students.Remove(student);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
