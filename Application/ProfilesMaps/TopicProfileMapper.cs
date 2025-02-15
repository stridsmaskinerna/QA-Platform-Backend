

using AutoMapper;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.ProfilesMaps
{
    internal class TopicProfileMapper : Profile
    {
        public TopicProfileMapper()
        {
            CreateMap<Topic, TopicDTO>().ReverseMap();
        }
    }
}
