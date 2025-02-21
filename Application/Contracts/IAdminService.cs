using Domain.DTO.Query;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Contracts;

public interface IAdminService
{
    Task AssignTeacherRoleToUser(string Id);
    Task<User?> BlockUserByIdAsync(string Id);
    Task<(IEnumerable<UserDetailsDTO> usersDTO, int totalItemCount)> GetUsersAsync(PaginationDTO paginationDTO, string searchString);
}
