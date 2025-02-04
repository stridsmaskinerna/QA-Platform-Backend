using System.Data.SqlTypes;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly QAPlatformContext _dbContext;

        public TagRepository(QAPlatformContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tag?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Tags.FindAsync(id);
        }

        public async Task<Tag?> GetByValueAsync(string value)
        {
            return await _dbContext.Tags.FirstOrDefaultAsync(t => t.Value == value);
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _dbContext.Tags.ToListAsync();
        }

        public async Task<IEnumerable<Tag>> GetFilteredList(string value)
        {
            var test = await _dbContext.Tags
                                   .Where(t => EF.Functions.Like(t.Value, $"%{value}%"))
                                   .ToListAsync();
            return test;
        }

        public async Task<Tag> AddAsync(Tag tag)
        {
            _dbContext.Tags.Add(tag);
            await _dbContext.SaveChangesAsync();
            return tag;
        }

        public async Task UpdateAsync(Tag tag)
        {
            _dbContext.Tags.Update(tag);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var tag = await _dbContext.Tags.FindAsync(id);
            if (tag != null)
            {
                _dbContext.Tags.Remove(tag);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteUnusedTagsAsync(
            Tag tag
        )
        {
            var isTagUsed = await _dbContext.Questions
                .AnyAsync(q => q.Tags.Any(t => t.Id == tag.Id));

            if (!isTagUsed)
            {
                _dbContext.Tags.Remove(tag);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsTagValueTakenAsync(string tagValue)
        {
            return await _dbContext.Tags.AnyAsync(t => t.Value == tagValue);
        }
    }
}
