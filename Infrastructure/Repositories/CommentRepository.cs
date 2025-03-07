using System.Data;
using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly QAPlatformContext _dbContext;

        public CommentRepository(QAPlatformContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Comments.FindAsync(id);
        }

        public async Task<IEnumerable<Comment>> GetCommentsByAnswerIdAsync(Guid answerId)
        {
            return await _dbContext.Comments.Where(c => c.AnswerId == answerId).ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(Guid userId)
        {
            return await _dbContext.Comments.Where(c => c.UserId == userId.ToString()).ToListAsync();
        }

        public async Task<Comment> AddAsync(Comment comment)
        {
            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();
            return comment;
        }

        public async Task UpdateAsync(Comment comment)
        {
            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment comment)
        {
            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
