using Application.Contracts;
using Domain.Constants;
using Domain.Contracts;
using Domain.DTO.Response;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class VoteService : BaseService, IVoteService
{
    private readonly IAnswerVoteRepository _answerVoteRepository;
    private readonly IAnswerRepository _answerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IServiceManager _sm;

    public VoteService(
        IAnswerVoteRepository answerVoteRepository,
        IAnswerRepository answerRepository,
        IUserRepository userRepository,
        IServiceManager sm
    )
    {
        _answerVoteRepository = answerVoteRepository;
        _answerRepository = answerRepository;
        _userRepository = userRepository;
        _sm = sm;
    }

    public async Task CastVote(
        Guid answerId,
        string vote
    )
    {
        var answer = await _answerRepository.GetByIdAsync(answerId);

        if (answer == null)
        {
            NotFound($"Answer with provided id {answerId} not found");
        }

        var userId = _sm.TokenService.GetUserId();

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            BadRequest();
        }

        var answerVote = await _answerVoteRepository.GetAsync(answerId, userId);


        if (answerVote == null && vote == VoteType.NEUTRAL)
        {
            var newAnswerVote = new AnswerVotes()
            {
                AnswerId = answerId,
                UserId = userId,
                Answer = answer,
                User = user,
                // Vote = vote
            };
            await _answerVoteRepository.AddAsync(newAnswerVote);
        }
        else
        {
            if (vote == VoteType.LIKE || vote == VoteType.DISLIKE)
            {
                //var voteAsBool = vote switch
                //{
                //    VoteType.LIKE => true,
                //    VoteType.DISLIKE => false,
                //    _ => null
                //};
                // answerVote.Vote = vote
                await _answerVoteRepository.UpdateAsync(answerVote);
            }
            else
            {
                await _answerVoteRepository.DeleteAsync(answerId, userId);
            }
        }
    }
}
