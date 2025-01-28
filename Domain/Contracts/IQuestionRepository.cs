using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question?> GetByIdAsync(Guid id);
        Task<List<Question>> GetAllAsync();
        Task<Question> AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(Guid id);

    }
}
