using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Contracts
{
    public interface ITopicService
    {
        Task<TopicDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<TopicDTO>> GetAllAsync();
        Task<TopicDTO> AddAsync(TopicDTO topic);
        Task UpdateAsync(Guid id, TopicDTO topic);
        Task DeleteAsync(Guid id);
    }
}
