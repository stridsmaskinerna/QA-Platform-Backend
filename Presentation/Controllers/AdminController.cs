using Application.Contracts;
using Domain.Constants;
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
    private bool isAdmin;

    public AdminController(IServiceManager sm) => _sm = sm;

    [HttpPut("blockuser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BlockUserById([FromQuery] string Id)
    {
        var blockedUSer = await _sm.AdminService.BlockUserByIdAsync(Id);
        return Ok(blockedUSer);
    }

}
