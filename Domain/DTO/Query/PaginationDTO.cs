namespace Domain.DTO.Query;

public class PaginationDTO
{
    public int Limit { get; set; } = 20;

    public int PageNr { get; init; } = 1;
}
