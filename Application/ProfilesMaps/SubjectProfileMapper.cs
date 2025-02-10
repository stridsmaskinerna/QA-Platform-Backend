using AutoMapper;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.ProfilesMaps;

public class SubjectProfileMapper : Profile
{
    public SubjectProfileMapper()
    {
        CreateMap<Subject, SubjectDTO>().ReverseMap();
        CreateMap<SubjectForCreationDTO, Subject>().ReverseMap();
    }
}
