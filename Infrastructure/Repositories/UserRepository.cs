using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// A repository class that wrap UserManager<User> used
/// to improve testing.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(
        UserManager<User> userManager
    )
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<User>> GetTeachersBySubjectIdAsync(Guid subjectId)
    {
        return await _userManager.Users
            .Where(u => u.Subjects.Any(s => s.Id == subjectId))
            .ToListAsync();
    }
}
