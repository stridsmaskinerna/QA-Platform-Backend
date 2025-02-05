namespace Application.Contracts;

public interface IVoteService
{
    Task CastVoteAsync(Guid answerId, bool? voteAsBoolean);
}
