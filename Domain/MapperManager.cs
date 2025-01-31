using System.Runtime.InteropServices;
using AutoMapper;
using Domain.DTO.Response;
using Domain.Entities;

namespace Domain
{
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



            CreateMap<Question, QuestionDTO>();
            // .ForMember(dest => dest.TopicName, opt => opt.Ignore())
            // .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src=> src.))
            // .ForMember(dest => dest.SubjectCode, opt => opt.Ignore())
            // .ForMember(dest => dest.SubjectId, opt => opt.Ignore())
            // .ForMember(dest => dest.AnswerCount, opt => opt.MapFrom(src => src.Answers.Count))
            // .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Value).ToList()));

            CreateMap<Subject, SubjectDTO>().ReverseMap(); //used for Subject creation too

            CreateMap<Comment, CommentDTO>().ReverseMap(); //used to create the commento too

            CreateMap<Answer, AnswerDTO>();

        }
    }
}
