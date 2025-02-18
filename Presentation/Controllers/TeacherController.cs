using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Authorize(Roles = $"{DomainRoles.TEACHER}")]
[Route("api/teacherconsole")]
[Produces("application/json")]

public class TeacherController : ControllerBase
{

    private readonly IServiceManager _sm;

    public TeacherController(IServiceManager sM)
    {
        _sm = sM;
    }

    [HttpPut("blockuser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> BlockUserById([FromQuery] string Id) {
        var blockedUSer = await _sm.TeacherService.BlockUserByIdAsync(Id);
        return Ok(blockedUSer);
    }
}
