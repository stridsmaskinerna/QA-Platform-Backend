using System.Text.Json;
using Application.Contracts;
using Domain.Constants;
using Domain.DTO.Header;
using Domain.DTO.Query;
using Domain.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Authorize(Roles = $"{DomainRoles.ADMIN}")]
[Route("api/admin")]
[Produces("application/json")]

public class AdminController : ControllerBase
{
    private readonly IServiceManager _sm;

    public AdminController(IServiceManager sm) => _sm = sm;

    [HttpPut("blockuser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BlockUserById([FromQuery] string Id)
    {
        var blockedUSer = await _sm.AdminService.BlockUserByIdAsync(Id);
        return Ok(blockedUSer);
    }

    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserDetailsDTO>>> GetUsers
    (
        [FromQuery] PaginationDTO paginationDTO,
        [FromQuery] string searchString)
    {
        var (usersDTO, totalItemCount) = await _sm.AdminService.GetUsersAsync(paginationDTO, searchString);

        var paginationMeta = new PaginationMetaDTO()
        {
            PageNr = paginationDTO.PageNr,
            Limit = paginationDTO.Limit,
            TotalItemCount = totalItemCount
        };

        Response.Headers.Append(
            CustomHeaders.Pagination,
            JsonSerializer.Serialize(paginationMeta)
        );

        return Ok(usersDTO);
    }


}
