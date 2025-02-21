using Domain.Contracts;
using Domain.DTO.Query;

namespace Infrastructure.Repositories;

public abstract class BaseRepository : IBaseRepository
{
    public IQueryable<T> ApplyPagination<T>(IQueryable<T> queryable, PaginationDTO paginationDTO)
    {
        return queryable
          .Skip(paginationDTO.Limit * (paginationDTO.PageNr - 1))
          .Take(paginationDTO.Limit);
    }
}