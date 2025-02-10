using System.Data.Entity;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// A repository class that wrap and extend UserManager<User> used
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

    public IEnumerable<User> GetTeachersBySubjectIdAsync(Guid subjectId)
    {
        return _userManager.Users
            .Where(u => u.Subjects.Any(s => s.Id == subjectId))
            .ToList();
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

    public User? GetUserByMail(string mail) {

        User? us = _userManager.Users.Where(user => user.Email == mail).FirstOrDefault();

        return us;

    }
}
