using Application.Contracts;
using Domain.Contracts;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Infrastructure.Contexts;

namespace Application.Services;

public class SubjectService : BaseService, ISubjectService
{
    private readonly IRepositoryManager _rm;
    private readonly IServiceManager _sm;
    private QAPlatformContext _dbContext;

    public SubjectService(IRepositoryManager rm, IServiceManager sm, QAPlatformContext dbContext)
    {
        _rm = rm;
        _sm = sm;
        _dbContext = dbContext;
    }
    public async Task<SubjectDTO> AddAsync(SubjectForCreationDTO subject)
    {
        Console.WriteLine("prova : ");
        try
        {
            Subject sbjObj = new();
            sbjObj.Name = subject.Name;
            sbjObj.SubjectCode = subject.SubjectCode;
            foreach (string mail in subject.Teachers)
            {
                User? choosenTeacher = _dbContext.Users.Where(user => user.Email == mail).FirstOrDefault();
                if (choosenTeacher != null)
                    sbjObj.Teachers.Add(choosenTeacher);
            }
            sbjObj.Topics = [];
            return _sm.Mapper.Map<SubjectDTO>(await _rm.SubjectRepository.AddAsync(sbjObj));

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

    }


    public async Task DeleteAsync(Guid id)
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

    public async Task UpdateAsync(Guid Id, SubjectForCreationDTO subject)
    {
        try
        {

            var sbjObj = await _rm.SubjectRepository.GetByIdAsync(Id);

            if (sbjObj == null)
            {
                NotFound($"No answer with id {Id} exist.");
            }

            sbjObj.Name = subject.Name;
            sbjObj.SubjectCode = subject.SubjectCode;
            foreach (string mail in subject.Teachers)
            {
                User? choosenTeacher = _dbContext.Users.Where(user => user.Email == mail).FirstOrDefault();
                if (choosenTeacher != null)
                    sbjObj.Teachers.Add(choosenTeacher);
            }

            await _rm.SubjectRepository.UpdateAsync(sbjObj);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return; 
        }
    }
}
