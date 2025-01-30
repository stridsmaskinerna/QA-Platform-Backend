using AutoMapper;
using Domain.DTO.Response;
using Domain.Entities;

namespace Domain
{
    public class MapperManager : Profile
    {
        public MapperManager()
        {

            CreateMap<User, UserDTO>();
            CreateMap<User, UserWithEmailDTO>();
            CreateMap<User, UserDetailsDTO>().ReverseMap();  //used for User creation too

            CreateMap<Question, QuestionDTO>();
            CreateMap<Question, QuestionDetailedDTO>();

            CreateMap<Subject, SubjectDTO>().ReverseMap(); //used for Subject creation too

            CreateMap<Comment, CommentDTO>().ReverseMap(); //used to create the commento too

            CreateMap<Answer, AnswerDTO>();

        }
    }
}
