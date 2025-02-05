namespace Application.Contracts
{
    public interface IVoteService
    {
        Task CastVote(Guid answerId, string vote);
    }
}
