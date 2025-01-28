using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface ITagRepository
    {
        Task<Tag?> GetByIdAsync(Guid id);
        Task<Tag?> GetByValueAsync(string value);
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag> AddAsync(Tag tag);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(Guid id);
        Task<bool> IsTagValueTakenAsync(string tagValue);
        // Task<List<Question>> GetQuestionsByTagIdAsync(Guid tagId);
    }
}
