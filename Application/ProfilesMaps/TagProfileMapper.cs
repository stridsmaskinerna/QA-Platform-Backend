using AutoMapper;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.ProfilesMaps;

public class TagProfileMapper : Profile
{
    public TagProfileMapper()
    {

        CreateMap<Tag, TagStandardDTO>().ReverseMap();
    }
}
