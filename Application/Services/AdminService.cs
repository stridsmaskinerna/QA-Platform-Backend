using Application.Contracts;
using Domain.Contracts;
using Domain.DTO.Query;
using Domain.DTO.Response;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Application.Services;

public class AdminService : BaseService, IAdminService
{
    public readonly IRepositoryManager _rm;
    public readonly IServiceManager _sm;

    public AdminService(IRepositoryManager rm, IServiceManager sm)
    {
        _rm = rm;
        _sm = sm;
    }

    public async Task AssignTeacherRoleToUser(string Id)
    {
        await _rm.UserRepository.AddUserRoleTeacher(Id);
    }

    public async Task<User?> BlockUserByIdAsync(string Id)
    {
        var user = await _rm.UserRepository.BlocKUserById(Id, true);
        if (user == null)
            NotFound("The user is not in the database");
        return user;
    }

    public async Task<(IEnumerable<UserDetailsDTO> usersDTO, int totalItemCount)> GetUsersAsync(PaginationDTO paginationDTO, string? searchString)
    {
        var (users, totalItemCount) = await _rm.UserRepository.GetUsersAsync(paginationDTO, searchString);
        var usersDTO = _sm.Mapper.Map<IEnumerable<UserDetailsDTO>>(users);
        return (usersDTO, totalItemCount);

    }
}
