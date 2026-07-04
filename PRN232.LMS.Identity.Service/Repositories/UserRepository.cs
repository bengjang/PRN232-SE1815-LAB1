using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Identity.Service.Entities;

namespace PRN232.LMS.Identity.Service.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}

public class UserRepository : IUserRepository
{
    private readonly Data.IdentityDbContext _context;

    public UserRepository(Data.IdentityDbContext context) => _context = context;

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
}
