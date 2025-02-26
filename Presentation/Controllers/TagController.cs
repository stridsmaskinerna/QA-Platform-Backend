using System.Text.Json;
using Application.Contracts;
using Domain.Constants;
using Domain.DTO.Header;
using Domain.DTO.Query;
using Domain.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;
using Swashbuckle.AspNetCore.Annotations;
namespace Presentation.Controllers;

[ApiController]
[Route("api/tags")]
[Produces("application/json")]
public class TagController : ControllerBase
{
    private readonly IServiceManager _sm;

    public TagController(IServiceManager sM)
    {
        _sm = sM;
    }

    [HttpGet]
    [Authorize(Roles = $"{DomainRoles.ADMIN}, {DomainRoles.TEACHER}, {DomainRoles.USER}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperationFilter(typeof(CustomHeadersOperationFilter))]
    public async Task<ActionResult<IEnumerable<TagStandardDTO>>> GetFilteredTagListByValue(
        [FromQuery] string? subTagValue,
        [FromQuery] PaginationDTO? paginationDTO
    )
    {
        var paginationDTODerived = paginationDTO == null
            ? new PaginationDTO() : paginationDTO;

        var (tags, totalItemCount) = await _sm.TagService.GetAllAsync(
            paginationDTODerived, subTagValue);

        var paginationMeta = new PaginationMetaDTO()
        {
            PageNr = paginationDTODerived.PageNr,
            Limit = paginationDTODerived.Limit,
            TotalItemCount = totalItemCount
        };

        Response.Headers.Append(
            CustomHeaders.Pagination,
            JsonSerializer.Serialize(paginationMeta)
        );

        return Ok(tags);

    }

    [HttpDelete]
    [Authorize(Roles = $"{DomainRoles.ADMIN}, {DomainRoles.TEACHER}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTagAsync(Guid id)
    {
        await _sm.TagService.DeleteAsync(id);
        return Ok(new { message = "Tag removed: ", data = id });
    }

}
