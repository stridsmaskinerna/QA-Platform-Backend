using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.Contracts;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Services;

public class TopicService : BaseService, ITopicService
{
    public readonly IRepositoryManager _rm;
    private readonly IServiceManager _sm;

    public TopicService(
        IRepositoryManager rm,
        IServiceManager sm
     )
    {
        _rm = rm;
        _sm = sm;
    }
    public async Task<TopicDTO> AddAsync(TopicDTO topicDTO)
    {
        var topic = _sm.Mapper.Map<Topic>(topicDTO);
        var addedTopic = await _rm.TopicRepository.AddAsync(topic);
        return _sm.Mapper.Map<TopicDTO>(addedTopic);
    }

    public async Task DeleteAsync(Guid id)
    {
        var topic = await _rm.TopicRepository.GetByIdAsync(id);
        if (topic == null)
        {
            NotFound("Topic not found in the database");
        }

        await _rm.TopicRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<TopicDTO>> GetAllAsync()
    {
        var topics = await _rm.TopicRepository.GetAllAsync();
        return _sm.Mapper.Map<IEnumerable<TopicDTO>>(topics);
    }

    public async Task<TopicDTO> GetByIdAsync(Guid id)
    {
        var topic = await _rm.TopicRepository.GetByIdAsync(id);
        if (topic == null)
        {
            NotFound("Topic not found in the database");
        }
        return _sm.Mapper.Map<TopicDTO>(topic);
    }

    public async Task UpdateAsync(Guid id, TopicDTO topicDTO)
    {
        var topic = await _rm.TopicRepository.GetByIdAsync(id);
        if (topic == null)
        {
            NotFound("Topic not found in database");
            return;
        }
        _sm.Mapper.Map(topicDTO, topic);
        await _rm.TopicRepository.UpdateAsync(topic);
    }
}
