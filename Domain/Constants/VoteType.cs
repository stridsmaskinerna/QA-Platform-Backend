namespace Domain.Constants;

public static class VoteType
{
    public const string LIKE = "like";
    public const string DISLIKE = "dislike";
    public const string NEUTRAL = "neutral";
    public static string[] ALL_TYPES = [LIKE, DISLIKE, NEUTRAL];
}
