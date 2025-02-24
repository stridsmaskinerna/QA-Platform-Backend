using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Contracts;

public interface ITopicService
{
    Task<TopicDTO> GetByIdAsync(Guid id);
    Task<IEnumerable<TopicDTO>> GetAllAsync();
    Task<TopicDTO> AddAsync(TopicForCreationDTO topic);
    Task UpdateAsync(Guid id, TopicDTO topic);
    Task<Topic?> DeleteAsync(Guid id);
}
