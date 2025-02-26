using Domain.DTO.Query;

namespace Domain.Contracts;


public interface IBaseRepository
{
    IQueryable<T> ApplyPagination<T>(IQueryable<T> queryable, PaginationDTO paginationDTO);
}
