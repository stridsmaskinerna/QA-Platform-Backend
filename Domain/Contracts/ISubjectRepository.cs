using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface ISubjectRepository
    {
        Task<List<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(Guid id);
        Task<Subject?> GetByNameAsync(string name);
        Task<Subject?> GetByCodeAsync(string code);
        Task<Subject> AddAsync(Subject subject);
        Task UpdateAsync(Subject subject);
        Task DeleteAsync(Guid id);

        // Task<List<Topic>> GetTopicsBySubjectIdAsync(Guid subjectId);
    }
}
