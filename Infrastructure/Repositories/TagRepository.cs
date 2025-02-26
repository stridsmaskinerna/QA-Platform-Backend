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
            PaginationDTO paginationDTO, string? searchString
        )
        {
            var query = _dbContext.Tags.AsQueryable();

            query = query.Pipe(q => ApplySearchFilter(q, searchString));

            var totalItemCount = await query.CountAsync();


            query = query.Pipe(q =>
                    ApplyPagination(q, paginationDTO));

            return (Tags: await query.ToListAsync(), TotalItemCount: totalItemCount);

        }

        private IQueryable<Tag> ApplySearchFilter(IQueryable<Tag> queryable, string? searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return queryable;
            }

            var searchStrings = searchString.Split(
                //Build error (on Mac) if not explicitly typing the space, i.e new char[] { ' ' } or new string[] { " " },
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in searchStrings)
            {
                var query = $"%{word}%";

                queryable = queryable.Where(t => EF.Functions.ILike(t.Value, query));
            }

            return queryable;
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
