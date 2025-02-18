using Domain.Entities;

namespace Domain.Contracts;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetTeachersBySubjectIdAsync(Guid subjectId);
    Task<User?> GetUserByMailAsync(string mail);
    Task<User?> ValidateUserCredential(string? email, string? password);
    Task ChangeUserRoleToTeacher(string Id);
    Task<User?> BlocKUserById(string Id);
}
