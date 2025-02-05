using Application.Contracts;
using Domain.Constants;
using Domain.Contracts;
using Infrastructure.Repositories;

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

        var answerVoteEntry = await _answerVoteRepository.GetAsync(
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
        await _answerVoteRepository.AddAsync(answerVote);
    }

    private async Task UpdateAnswerVote(
        bool vote,
        AnswerVotes answerVote
    )
    {
        answerVote.Vote = vote;
        await _answerVoteRepository.UpdateAsync(answerVote);
    }

    private async Task DeleteAnswerVote(Guid answerId, string userId)
    {
        await _answerVoteRepository.DeleteAsync(answerId, userId);
    }
}
