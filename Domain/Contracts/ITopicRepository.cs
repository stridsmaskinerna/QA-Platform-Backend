using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface ITopicRepository
    {
        Task<Topic?> GetByIdAsync(Guid id);
        Task<Topic?> GetByNameAsync(string name);
        Task<IEnumerable<Topic>> GetTopicsBySubjectIdAsync(Guid subjectId);
        Task<IEnumerable<Topic>> GetAllAsync();
        Task<Topic> AddAsync(Topic topic);
        Task UpdateAsync(Topic topic);
        Task DeleteAsync(Guid id);
    }
}
