using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question?> GetByIdAsync(Guid id);
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question> AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(Guid id);

    }
}
