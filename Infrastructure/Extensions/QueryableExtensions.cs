namespace Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Pipe<T>(
        this IQueryable<T> source,
        Func<IQueryable<T>, IQueryable<T>> func
    )
    {
        return func(source);
    }
}
