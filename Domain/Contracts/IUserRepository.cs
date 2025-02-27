using Domain.DTO.Query;
using Domain.Entities;

namespace Domain.Contracts;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetTeachersBySubjectIdAsync(Guid subjectId);
    Task<User?> GetUserByMailAsync(string mail);
    Task<User?> ValidateUserCredential(string? email, string? password);
    Task AddUserRoleTeacher(string Id);
    Task<User?> BlocKUserById(string Id, bool isAdmin = false);
    Task<(IEnumerable<User> users, int totalItemCount)> GetUsersAsync(PaginationDTO paginationDTO, string? searchString);

}
