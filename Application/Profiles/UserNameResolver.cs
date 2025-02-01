using Application.Services;
using AutoMapper;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

public class UserNameResolver : IValueResolver<Answer, AnswerDTO, string>
{
    private readonly IServiceManager _sm;

    public UserNameResolver(IServiceManager sm)
    {
        _sm = sm;
    }

    public string Resolve(
        Answer source,
        AnswerDTO destination,
        string destMember,
        ResolutionContext context
    )
    {
        return _sm.TokenService.GetUserName();
    }
}
