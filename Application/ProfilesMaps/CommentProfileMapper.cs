using AutoMapper;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.ProfilesMaps;

public class CommentProfileMapper : Profile
{
    public CommentProfileMapper()
    {
        CreateMap<Comment, CommentDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

        CreateMap<CommentForCreationDTO, Comment>();

        CreateMap<CommentForPutDTO, Comment>();

        CreateMap<Comment, CommentDTO>();
    }
}
