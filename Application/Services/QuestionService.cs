using Domain.Contracts;
using Domain.DTO.Query;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Services;

public class QuestionService : BaseService, IQuestionService
{
    private readonly IQuestionRepository _qr;
    private readonly IServiceManager _sm;

    public QuestionService(
        IQuestionRepository qr,
        IServiceManager sm
    )
    {
        _qr = qr;
        _sm = sm;
    }

    public async
    Task<(IEnumerable<Question> Questions, int TotalItemCount)>
    GetItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO,
        bool onlyPublic = true
    )
    {
        return await _qr.GetItemsAsync(paginationDTO, searchDTO, onlyPublic);
    }

    public async Task<QuestionDetailedDTO> GetByIdAsync(Guid id)
    {
        var question = await _qr.GetByIdAsync(id);

        if (question == null)
        {
            NotFound();
        }

        return _sm.Mapper.Map<QuestionDetailedDTO>(question);
    }

    // TODO Handle Tags
    // 1. Create New Tags.
    // 2. If Tags exist in database do not create.
    // 3. Handle QuestionTag connection table.
    public async Task<QuestionDTO> AddAsync(QuestionForCreationDTO questionDTO)
    {
        var question = _sm.Mapper.Map<Question>(questionDTO);

        if (question == null)
        {
            BadRequest("Invalid Question body");
        }

        question.UserId = _sm.TokenService.GetUserId();
        var createdQuestion = await _qr.AddAsync(question);

        var createdQuestionDTO = _sm.Mapper.Map<QuestionDTO>(createdQuestion);

        createdQuestionDTO.UserName = _sm.TokenService.GetUserName();
        return createdQuestionDTO;
    }

    // TODO Handle Tags
    // 1. Create New Tags.
    // 2. If Tags exist in database do not create.
    // 3. Handle QuestionTag connection table.
    public async Task UpdateAsync(Guid id, QuestionForPutDTO questionDTO)
    {
        var question = await _qr.GetByIdAsync(id);
        if (question == null)
        {
            NotFound($"No answer with id {id} exist.");
        }

        var updated = _sm.Mapper.Map(questionDTO, question);

        await _qr.CompleteAsync(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var question = await _qr.GetByIdAsync(id);

        if (question == null)
        {
            NotFound();
        }

        await _qr.DeleteAsync(id);
    }
}

