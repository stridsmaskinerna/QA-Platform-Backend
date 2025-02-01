using Application.Services;
using AutoMapper;
using Domain.DTO.Request;
using Domain.Entities;

public class UserIdResolver : IValueResolver<AnswerForCreationDTO, Answer, string>
{
    private readonly IServiceManager _sm;

    public UserIdResolver(IServiceManager sm)
    {
        _sm = sm;
    }

    public string Resolve(
        AnswerForCreationDTO source,
        Answer destination,
        string destMember,
        ResolutionContext context
    )
    {
        return _sm.TokenService.GetUserId();
    }
}
