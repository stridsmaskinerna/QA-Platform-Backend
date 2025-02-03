using AutoMapper;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.ProfilesMaps;

public class UserProfileMapper : Profile
{

    public UserProfileMapper() {
        CreateMap<User, UserDTO>();
        CreateMap<User, UserWithEmailDTO>();
        CreateMap<User, UserDetailsDTO>().ReverseMap();
    }
}
