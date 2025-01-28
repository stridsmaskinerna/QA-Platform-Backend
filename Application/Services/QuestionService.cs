using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _repository;

    public QuestionService(IQuestionRepository rp)
    {
        _repository = rp;
    }
    public async Task<IEnumerable<Question>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Question?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }




    public async Task<Question> AddAsync(Question question)
    {

        return await _repository.AddAsync(question);
    }

    public async Task UpdateAsync(Question question)
    {
        await _repository.UpdateAsync(question);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    Task<List<Question>> IQuestionService.GetAllAsync()
    {
        throw new NotImplementedException();
    }
}

