using Domain.Constants;
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

    public async Task ChangeUserRoleToTeacher(string Id)
    {
        var user = await _userManager.FindByIdAsync(Id);
        if (user != null)
        {
            await _userManager.AddToRoleAsync(user, DomainRoles.TEACHER);
        }
    }

    public async Task<User?> BlocKUserById(string Id, bool isAdmin = false)
    {
        var user = await _userManager.FindByIdAsync(Id);
        if (user == null) return null;

        if (isAdmin || !(await _userManager.GetRolesAsync(user)).Contains(DomainRoles.TEACHER))
        {
            user.IsBlocked = !user.IsBlocked;
            await _dbContext.SaveChangesAsync();
            return user;
        }

        return null;

    }
}
