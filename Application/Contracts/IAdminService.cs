using Domain.Entities;

namespace Application.Contracts;

public interface IAdminService
{
    Task AssignTeacherRoleToUser(string Id);
    Task<User?> BlockUserByIdAsync(string Id);
}
