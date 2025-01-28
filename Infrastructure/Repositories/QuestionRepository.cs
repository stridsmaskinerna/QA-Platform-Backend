using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly QAPlatformContext _dbContext;

        public QuestionRepository(QAPlatformContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _dbContext.Questions.ToListAsync();
        }

        public async Task<Question?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Questions.FindAsync(id);
        }




        public async Task<Question> AddAsync(Question question)
        {
            _dbContext.Questions.Add(question);
            await _dbContext.SaveChangesAsync();
            return question;
        }

        public async Task UpdateAsync(Question question)
        {
            _dbContext.Questions.Update(question);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var question = await _dbContext.Questions.FindAsync(id);
            if (question != null)
            {
                _dbContext.Questions.Remove(question);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
