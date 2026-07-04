using PRN232.LMS.Contracts.Exceptions;
using PRN232.LMS.Student.Service.Models;
using StudentEntity = PRN232.LMS.Student.Service.Entities.Student;
using PRN232.LMS.Student.Service.Repositories;

namespace PRN232.LMS.Student.Service.Services;

public interface IStudentService
{
    Task<List<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StudentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<StudentDto> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);
    Task<StudentDto> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository) => _repository = repository;

    public async Task<List<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var students = await _repository.GetAllAsync(cancellationToken);
        return students.Select(MapToDto).ToList();
    }

    public async Task<StudentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _repository.GetByIdAsync(id, cancellationToken);
        if (student is null)
            throw new BusinessException($"Student with id {id} was not found.", 404);
        return MapToDto(student);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        _repository.ExistsAsync(id, cancellationToken);

    public async Task<StudentDto> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new StudentEntity
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            DateOfBirth = request.DateOfBirth
        };
        var created = await _repository.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    public async Task<StudentDto> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Student with id {id} was not found.", 404);

        entity.FullName = request.FullName.Trim();
        entity.Email = request.Email.Trim();
        entity.DateOfBirth = request.DateOfBirth;
        await _repository.UpdateAsync(entity, cancellationToken);
        return MapToDto(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessException($"Student with id {id} was not found.", 404);
        await _repository.DeleteAsync(entity, cancellationToken);
    }

    private static StudentDto MapToDto(StudentEntity student) => new()
    {
        StudentId = student.StudentId,
        FullName = student.FullName,
        Email = student.Email,
        DateOfBirth = student.DateOfBirth
    };
}
