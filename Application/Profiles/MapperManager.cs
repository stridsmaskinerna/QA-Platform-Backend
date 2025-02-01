using AutoMapper;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Profiles;

// TODO Crate a Profile class for every resource, e.g., AnswerProfile
public class MapperManager : Profile
{
    public MapperManager()
    {
        // QAPlatformContext _context = new QAPlatformContext();

        CreateMap<User, UserDTO>();
        CreateMap<User, UserWithEmailDTO>();
        CreateMap<User, UserDetailsDTO>().ReverseMap();  //used for User creation too

        // CreateMap<Question, QuestionDTO>();
        CreateMap<Question, QuestionDetailedDTO>();

        CreateMap<Question, QuestionDTO>()
        .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Name))
        .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Topic.Subject.Name))
        .ForMember(dest => dest.AnswerCount, opt => opt.MapFrom(src => src.Answers.Count))
        .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Topic.Subject.SubjectCode))
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
        .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Topic.Subject.Id))
        .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Value).ToList()));

        CreateMap<Subject, SubjectDTO>().ReverseMap(); //used for Subject creation too

        CreateMap<Comment, CommentDTO>().ReverseMap(); //used to create the comment too

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

