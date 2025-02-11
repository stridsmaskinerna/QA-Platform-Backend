using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly QAPlatformContext _dbContext;

        public SubjectRepository(QAPlatformContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _dbContext.Subjects
                                     .Include(us => us.Teachers)
                                     .Select(us => new Subject
                                     {
                                         Id = us.Id,
                                         Name = us.Name,
                                         SubjectCode = us.SubjectCode,
                                         Teachers = us.Teachers,
                                         Topics = us.Topics.Where(t => t.IsActive).ToList()
                                     }).ToListAsync();

        }


        public async Task<Subject?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Subjects.Include(us => us.Teachers).Include(us => us.Topics).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Subject?> GetByNameAsync(string name)
        {
            return await _dbContext.Subjects.Include(us => us.Teachers).Include(us => us.Topics).FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<Subject?> GetByCodeAsync(string code)
        {
            return await _dbContext.Subjects.FirstOrDefaultAsync(s => s.SubjectCode == code);
        }

        public async Task<Subject> AddAsync(Subject subject)
        {
            _dbContext.Subjects.Add(subject);
            await _dbContext.SaveChangesAsync();
            return subject;
        }

        public async Task UpdateAsync(Subject subject)
        {
            _dbContext.Subjects.Update(subject);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var subject = await _dbContext.Subjects.FindAsync(id);
            if (subject != null)
            {
                _dbContext.Subjects.Remove(subject);
                await _dbContext.SaveChangesAsync();
            }
        }

        //public async Task<List<Topic>> GetTopicsBySubjectIdAsync(Guid subjectId)
        //{
        //    return await _dbContext.Topics
        //        .Where(t => t.SubjectId == subjectId)
        //        .ToListAsync();
        //}
    }
}
