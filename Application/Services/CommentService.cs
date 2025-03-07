using Application.Contracts;
using Domain.Contracts;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Services;

public class CommentService : BaseService, ICommentService
{
    private readonly IRepositoryManager _rm;
    private readonly IServiceManager _sm;

    public CommentService(
        IRepositoryManager rm,
        IServiceManager sm
    )
    {
        _rm = rm;
        _sm = sm;
    }

    public static string MsgAddAsyncBadRequest() => "Could not create the new comment";

    public static string MsgNotFound(Guid id) => $"No comment with id {id} exist.";

    public async Task<CommentDTO> AddAsync(CommentForCreationDTO commentDTO)
    {
        var comment = _sm.Mapper.Map<Comment>(commentDTO);
        if (comment == null)
        {
            BadRequest(MsgAddAsyncBadRequest());
        }

        comment.UserId = _sm.TokenService.GetUserId();

        var createdcomment = await _rm.CommentRepository.AddAsync(comment);
        var createdcommentDTO = _sm.Mapper.Map<CommentDTO>(createdcomment);
        createdcommentDTO.UserName = _sm.TokenService.GetUserName();

        return createdcommentDTO;
    }

    public async Task UpdateAsync(Guid id, CommentForPutDTO commentDTO)
    {
        var comment = await _rm.CommentRepository.GetByIdAsync(id);
        if (comment == null)
        {
            NotFound(MsgNotFound(id));
        }

        var updated = _sm.Mapper.Map(commentDTO, comment);

        await _rm.CommentRepository.UpdateAsync(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var comment = await _rm.CommentRepository.GetByIdAsync(id);
        if (comment == null)
        {
            NotFound(MsgNotFound(id));
        }

        await _rm.CommentRepository.DeleteAsync(comment);
    }
}
