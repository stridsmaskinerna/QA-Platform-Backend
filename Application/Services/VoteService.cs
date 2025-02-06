using Application.Contracts;
using Domain.Constants;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class VoteService : BaseService, IVoteService
{
    private readonly IRepositoryManager _rm;
    private readonly IServiceManager _sm;
    private readonly UserManager<User> _um;

    public VoteService(
        IRepositoryManager rm,
        IServiceManager sm,
        UserManager<User> um
    )
    {
        _rm = rm;
        _sm = sm;
        _um = um;
    }

    public bool? GetVoteAsBoolean(string vote)
    {
        if (!VoteType.ALL_TYPES.Contains(vote))
        {
            BadRequest($"Invalid vote type, valid types are [{string.Join(", ", VoteType.ALL_TYPES)}]");
        }

        bool? voteAsBoolean = vote switch
        {
            VoteType.LIKE => true,
            VoteType.DISLIKE => false,
            _ => null
        };

        return voteAsBoolean;
    }

    public async Task CastVoteAsync(Guid answerId, bool? voteAsBoolean)
    {
        var answer = await _rm.AnswerRepository.GetByIdAsync(answerId);
        if (answer == null)
        {
            NotFound($"Answer with provided id {answerId} not found");
        }

        var userId = _sm.TokenService.GetUserId();
        var user = await _um.FindByIdAsync(userId);
        if (user == null)
        {
            Unauthorized($"INvalid authentication user could not be found");
        }

        var answerVoteEntry = await _rm.AnswerVoteRepository.GetAsync(
            answerId,
            userId
        );

        if (answerVoteEntry == null && voteAsBoolean != null)
        {
            await CreateNewAnswerVote(new AnswerVotes()
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
            await UpdateAnswerVote(voteAsBoolean.Value, answerVoteEntry);
        }
        else if (answerVoteEntry != null && voteAsBoolean == null)
        {
            await DeleteAnswerVote(answerId, userId);
        }
    }

    private async Task CreateNewAnswerVote(AnswerVotes answerVote)
    {
        await _rm.AnswerVoteRepository.AddAsync(answerVote);
    }

    private async Task UpdateAnswerVote(
        bool vote,
        AnswerVotes answerVote
    )
    {
        answerVote.Vote = vote;
        await _rm.AnswerVoteRepository.UpdateAsync(answerVote);
    }

    private async Task DeleteAnswerVote(Guid answerId, string userId)
    {
        await _rm.AnswerVoteRepository.DeleteAsync(answerId, userId);
    }
}
