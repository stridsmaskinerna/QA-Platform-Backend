using Application.Contracts;
using Domain.Contracts;
using Domain.Entities;

namespace Application.Services;

public class AdminService : BaseService, IAdminService
{
    public readonly IRepositoryManager _rm;

    public AdminService(IRepositoryManager rm) => _rm = rm;

    public async Task AssignTeacherRoleToUser(string Id)
    {
        await _rm.UserRepository.ChangeUserRoleToTeacher(Id);
    }

    public async Task<User?> BlockUserByIdAsync(string Id)
    {
        var user = await _rm.UserRepository.BlocKUserById(Id, true);
        if (user == null)
            NotFound("The user is not in the database");
        return user;
    }
}
