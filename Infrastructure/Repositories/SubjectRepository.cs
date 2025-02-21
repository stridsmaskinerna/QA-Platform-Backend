using System.Reflection.Metadata.Ecma335;
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

        public async Task<IEnumerable<Subject>> GetTeachersSubjectsAsync(
            string teacherId
        )
        {
            return await _dbContext.Subjects
                .Include(s => s.Teachers)
                .Where(s => s.Teachers.Any(t => t.Id == teacherId))
                .Select(us => new Subject
                {
                    Id = us.Id,
                    Name = us.Name,
                    SubjectCode = us.SubjectCode,
                    Teachers = us.Teachers,
                    Topics = us.Topics
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

        public async Task<Subject?> DeleteAsync(Guid id)
        {
            var subject = await _dbContext.Subjects
                                            .Include(s => s.Topics)
                                            .ThenInclude(t => t.Questions)
                                            .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null || subject.Topics.Any(t => t.Questions.Any())) return null;
           
            _dbContext.Subjects.Remove(subject);
            await _dbContext.SaveChangesAsync();
            return subject;
            
        }
    }
}
