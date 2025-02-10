using Domain.Entities;

namespace Domain.Contracts;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetTeachersBySubjectIdAsync(Guid subjectId);
    Task<User?> ValidateUserCredential(string? email, string? password);
    User? GetUserByMail(string mail);
}
