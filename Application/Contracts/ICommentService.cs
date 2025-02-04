using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Application.Contracts
{
    public interface ICommentService
    {
        Task<CommentDTO> AddAsync(CommentForCreationDTO answerDTO);
        Task DeleteAsync(Guid id);
        Task UpdateAsync(Guid id, CommentForPutDTO answerDTO);
    }
}
