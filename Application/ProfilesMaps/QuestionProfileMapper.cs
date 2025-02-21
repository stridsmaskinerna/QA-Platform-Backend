using AutoMapper;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.ProfilesMaps;

public class QuestionProfileMapper : Profile
{

    public QuestionProfileMapper()
    {

        CreateMap<Question, QuestionDetailedDTO>()
        .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Name))
        .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Topic.Subject.Name))
        .ForMember(dest => dest.AnswerCount, opt => opt.MapFrom(src => src.Answers.Count))
        .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Topic.Subject.SubjectCode))
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
        .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Topic.Subject.Id))
        .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Value).ToList()));

        CreateMap<Question, QuestionDTO>()
        .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Name))
        .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Topic.Subject.Name))
        .ForMember(dest => dest.AnswerCount, opt => opt.MapFrom(src => src.Answers.Count))
        .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Topic.Subject.SubjectCode))
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
        .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Topic.Subject.Id))
        .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Value).ToList()));

        CreateMap<QuestionForCreationDTO, Question>()
        .ForMember(dest => dest.Tags, opt => opt.Ignore())
        .AfterMap((src, dest, context) =>
        {
            dest.IsHidden = false;
            dest.Created = DateTime.UtcNow;
        });

        CreateMap<Question, QuestionForEditDTO>()
        .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Topic.SubjectId))
        .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Value).ToList()));

        CreateMap<QuestionForPutDTO, Question>()
        .ForMember(dest => dest.Tags, opt => opt.Ignore());
    }
}
