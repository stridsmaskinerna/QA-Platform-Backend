using Domain.Entities;

namespace Domain.Contracts;

public interface ISubjectRepository
{
    Task<IEnumerable<Subject>> GetAllAsync();
    Task<Subject?> GetByIdAsync(Guid id);
    Task<Subject?> GetByNameAsync(string name);
    Task<Subject?> GetByCodeAsync(string code);
    Task<Subject> AddAsync(Subject subject);
    Task UpdateAsync(Subject subject);
    Task DeleteAsync(Guid id);

    // Task<List<Topic>> GetTopicsBySubjectIdAsync(Guid subjectId);
}
