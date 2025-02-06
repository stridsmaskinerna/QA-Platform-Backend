using Application.Contracts;
using Domain.Contracts;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Services;

public class SubjectService : ISubjectService
{
    private readonly IRepositoryManager _rm;
    private readonly IServiceManager _sm;

    public SubjectService(IRepositoryManager rm, IServiceManager sm)
    {
        _rm = rm;
        _sm = sm;
    }
    public async Task<SubjectDTO> AddAsync(SubjectForCreationDTO subject)
    {
        Console.WriteLine("prova : ");
        try
        {
            var sbjObj = _sm.Mapper.Map<Subject>(subject);
            return _sm.Mapper.Map<SubjectDTO>(await _rm.SubjectRepository.AddAsync(sbjObj));

        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante il mapping: " + ex.Message);
            throw;
        }

    }


    public async Task Delete(Guid id)
    {
        await _rm.SubjectRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<SubjectDTO>> GetAllAsync()
    {
        return _sm.Mapper.Map<IEnumerable<SubjectDTO>>(await _rm.SubjectRepository.GetAllAsync());
    }

    public async Task<SubjectDTO> GetByCodeAsync(string code)
    {
        return _sm.Mapper.Map<SubjectDTO>(await _rm.SubjectRepository.GetByCodeAsync(code));
    }

    public async Task<SubjectDTO> GetByIdAsync(Guid id)
    {
        return _sm.Mapper.Map<SubjectDTO>(await _rm.SubjectRepository.GetByIdAsync(id));
    }

    public async Task<SubjectDTO> GetByNameAsync(string name)
    {
        return _sm.Mapper.Map<SubjectDTO>(await _rm.SubjectRepository.GetByNameAsync(name));
    }

    public async Task UpdateAsync(SubjectForCreationDTO subject)
    {
        Subject sbjObj = _sm.Mapper.Map<Subject>(subject);
        await _rm.SubjectRepository.UpdateAsync(sbjObj);
    }
}
