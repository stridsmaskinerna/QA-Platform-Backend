using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    // Infrastructure/Persistence/TopicRepository.cs

    public class TopicRepository : ITopicRepository
    {
        private readonly QAPlatformContext _dbContext;

        public TopicRepository(QAPlatformContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Topic?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Topics.FindAsync(id);
        }

        public async Task<Topic?> GetByNameAsync(string name)
        {
            return await _dbContext.Topics.FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<IEnumerable<Topic>> GetTopicsBySubjectIdAsync(Guid subjectId)
        {
            return await _dbContext.Topics.Where(t => t.SubjectId == subjectId).ToListAsync();
        }

        public async Task<IEnumerable<Topic>> GetAllAsync()
        {
            return await _dbContext.Topics.ToListAsync();
        }

        public async Task<Topic> AddAsync(Topic topic)
        {
            _dbContext.Topics.Add(topic);
            await _dbContext.SaveChangesAsync();
            return topic;
        }

        public async Task UpdateAsync(Topic topic)
        {
            _dbContext.Topics.Update(topic);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var topic = await _dbContext.Topics.FindAsync(id);
            if (topic != null)
            {
                _dbContext.Topics.Remove(topic);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> IsTopicNameTakenInSubjectAsync(string name, Guid subjectId)
        {
            return await _dbContext.Topics.AnyAsync(t => t.Name == name && t.SubjectId == subjectId);
        }
    }
}
