using AutoMapper;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.ProfilesMaps;

public class SubjectProfileMapper : Profile
{
    public SubjectProfileMapper()
    {

        CreateMap<Subject, SubjectDTO>().ReverseMap();
    }
}
