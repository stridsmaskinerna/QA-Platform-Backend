using Application.Contracts;
using Domain.Contracts;
using Domain.Entities;

namespace Application.Services;

public class TeacherService : BaseService, ITeacherService
{
    public readonly IRepositoryManager _rm;

    public TeacherService(IRepositoryManager rm)
    {
        _rm = rm;
    }

    public async Task AssignTeacherRoleToUser(string Id)
    {
        await _rm.UserRepository.ChangeUserRoleToTeacher(Id);
    }



    public async Task<User?> BlockUserByIdAsync(string Id)
    {
        var user = await _rm.UserRepository.BlocKUserById(Id);
        if (user == null)
            Forbidden("Action Forbidden. You have not permission to block a teacher");
        return user;
    }
}
