using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IAnswerRepository
    {
        Task<Answer?> GetByIdAsync(Guid id);
        Task<List<Answer>> GetAnswersByQuestionIdAsync(Guid questionId);
        Task<List<Answer>> GetAnswersByUserIdAsync(Guid userId);
        Task<Answer> AddAsync(Answer answer);
        Task UpdateAsync(Answer answer);
        Task DeleteAsync(Guid id);
        Task<int> GetAnswerCountByQuestionIdAsync(Guid questionId); 
    }
}
