using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// A repository class that wrap and extend UserManager<User> used
/// to improve testing.
/// </summary>
public class UserRepository : IUserRepository
{
    private UserManager<User> _userManager;
    private readonly QAPlatformContext _dbContext;

    public UserRepository(
        QAPlatformContext dbContext,
        UserManager<User> userManager
    )
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IEnumerable<User>> GetTeachersBySubjectIdAsync(Guid subjectId)
    {
        var teachers = await _dbContext.Users
            .Where(u => u.Subjects.Any(s => s.Id == subjectId))
            .ToListAsync();

        return teachers;
    }

    public async Task<User?> ValidateUserCredential(string? email, string? password)
    {
        var user = await _userManager.FindByEmailAsync(email!);
        if (user == null ||
            !await _userManager.CheckPasswordAsync(user, password!))
        {
            return null;
        };

        return user;
    }

    public async Task<User?> GetUserByMailAsync(string mail)
    {
        return await _dbContext.Users
            .Where(user => user.Email == mail)
            .FirstOrDefaultAsync();
    }
}
