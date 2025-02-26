using Domain.Contracts;
using Domain.DTO.Query;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TagRepository : BaseRepository, ITagRepository
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

        public async Task<(IEnumerable<Tag> Tags, int TotalItemCount)> GetAllAsync(
            PaginationDTO paginationDTO
        )
        {
            var query = _dbContext.Tags;
            var totalItemCount = await query.CountAsync();

            return (
                Tags: await query.Pipe(q =>
                    ApplyPagination(q, paginationDTO))
                    .ToListAsync(),
                TotalItemCount: totalItemCount
            );  
        }

        public async Task<IEnumerable<Tag>> GetFilteredList(string value)
        {
            var test = await _dbContext.Tags
                                   .Where(t => EF.Functions.ILike(t.Value, $"%{value}%"))
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
            IEnumerable<Tag> tags
        )
        {
            foreach (var tag in tags)
            {
                var isTagUsed = await _dbContext.Questions
                    .AnyAsync(q => q.Tags.Any(t => t.Id == tag.Id));

                if (!isTagUsed)
                {
                    _dbContext.Tags.Remove(tag);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsTagValueTakenAsync(string tagValue)
        {
            return await _dbContext.Tags.AnyAsync(t => t.Value == tagValue);
        }
    }
}
