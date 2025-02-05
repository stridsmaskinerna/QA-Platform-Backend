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

    public async Task CastVoteAsync(Guid answerId, bool? voteAsBoolean)
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
            Unauthorized($"INvalid authentication user could not be found");
        }

        var answerVoteEntry = await _answerVoteRepository.GetAsync(answerId, userId);

        if (voteAsBoolean != null && answerVoteEntry == null)
        {
            await HandleNewVote(new AnswerVotes()
            {
                AnswerId = answerId,
                UserId = userId,
                Answer = answer,
                User = user,
                Vote = voteAsBoolean.Value
            });
        }
        else if (answerVoteEntry != null && voteAsBoolean != null)
        {
            await HandleUpdatedVote(voteAsBoolean.Value, answerVoteEntry);
        }
        else if (answerVoteEntry != null && voteAsBoolean == null)
        {
            await HandleDeletedVote(answerId, userId);
        }
    }

    private async Task HandleNewVote(AnswerVotes answerVote)
    {
        await _answerVoteRepository.AddAsync(answerVote);
    }

    private async Task HandleUpdatedVote(
        bool vote,
        AnswerVotes answerVote
    )
    {
        answerVote.Vote = vote;
        await _answerVoteRepository.UpdateAsync(answerVote);
    }

    private async Task HandleDeletedVote(Guid answerId, string userId)
    {
        await _answerVoteRepository.DeleteAsync(answerId, userId);
    }
}
