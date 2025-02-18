using AutoMapper;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.ProfilesMaps;

internal class TopicProfileMapper : Profile
{
    public TopicProfileMapper()
    {
        CreateMap<Topic, TopicDTO>().ReverseMap();

        CreateMap<TopicForCreationDTO, Topic>();
    }
}
