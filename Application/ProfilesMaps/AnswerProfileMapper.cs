using AutoMapper;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.ProfilesMaps;

public class AnswerProfileMapper : Profile
{
    public AnswerProfileMapper()
    {

        CreateMap<Answer, AnswerDTO>();

        CreateMap<AnswerForCreationDTO, Answer>()
            .ForMember(d => d.UserId, o => o.MapFrom<UserIdResolver>())
            .AfterMap((s, d) =>
            {
                d.IsHidden = true;
                d.Created = DateTime.UtcNow;
            });

        CreateMap<AnswerForPutDTO, Answer>();
    }

}
