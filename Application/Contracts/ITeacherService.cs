
using Domain.Entities;

namespace Application.Contracts;

public interface ITeacherService
{
    Task<User?> BlockUserByIdAsync(string Id);
}
