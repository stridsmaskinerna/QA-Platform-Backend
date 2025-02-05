using AutoMapper;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.ProfilesMaps;

public class AnswerProfileMapper : Profile
{
    public AnswerProfileMapper()
    {
        CreateMap<Answer, AnswerDTO>()
        .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
        .ForMember(dest => dest.VoteCount, opt => opt.MapFrom(src => src.AnswerVotes.Count));

        CreateMap<Answer, AnswerDetailedDTO>()
        .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
        .ForMember(dest => dest.VoteCount, opt => opt.MapFrom(src => src.AnswerVotes.Count));

        CreateMap<AnswerForCreationDTO, Answer>()
            .AfterMap((s, d) =>
            {
                d.IsHidden = false;
                d.Created = DateTime.UtcNow;
            });

        CreateMap<AnswerForPutDTO, Answer>();
    }

}
