namespace Domain.DTO.Header;

public class PaginationMetaDTO
{
    public int Limit { get; init; } = 20;

    public int PageNr { get; init; } = 1;

    public required int TotalItemCount { get; init; }

    public int ExcludedItems { get; init; } = 0;

    public int TotalPageCount => Convert.ToInt32(Math.Ceiling(TotalItemCount / Convert.ToDouble(Limit)));
}
