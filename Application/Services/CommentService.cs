using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Services
{
    public class CommentService : BaseService, ICommentService 
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IServiceManager _sm;
        public CommentService
            (ICommentRepository commentRepository,
            IServiceManager sm,
            IAnswerRepository answerRepository)
        {
            _commentRepository = commentRepository;
            _sm = sm;
            _answerRepository = answerRepository;
        }
        public async Task<CommentDTO> AddAsync(CommentForCreationDTO commentDTO)
        {
            var comment = _sm.Mapper.Map<Comment>(commentDTO);
            if (comment == null)
            {
                BadRequest("Could not create the new comment");
            }
            //comment.UserId = "03ba7422-eaf0-4f16-a6f8-44170c9d0ad6";
            comment.UserId = _sm.TokenService.GetUserId();
            var createdcomment = await _commentRepository.AddAsync(comment);
            var createdcommentDTO = _sm.Mapper.Map<CommentDTO>(createdcomment);
            createdcommentDTO.UserName = _sm.TokenService.GetUserName();
            //createdcommentDTO.UserName = "Earnest Hammes";
            return createdcommentDTO;
        }

        public async Task DeleteAsync(Guid id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                NotFound($"No comment with id {id} exist.");
            }

            await _commentRepository.DeleteAsync(id);
        }

        public async Task UpdateAsync(Guid id, CommentForPutDTO commentDTO)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                NotFound($"No comment with id {id} exist.");
            }

            var updated = _sm.Mapper.Map(commentDTO, comment);

            await _commentRepository.UpdateAsync(updated);

        }
    }
}
