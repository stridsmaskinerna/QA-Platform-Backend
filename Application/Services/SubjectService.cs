using Application.Contracts;
using Domain.Contracts;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class SubjectService : BaseService, ISubjectService
{
    private readonly IRepositoryManager _rm;
    private readonly IServiceManager _sm;
    private readonly UserManager<User> _userManager;

    public SubjectService(IRepositoryManager rm, IServiceManager sm, UserManager<User> userManager)
    {
        _rm = rm;
        _sm = sm;
        _userManager = userManager;
    }

    public async Task<SubjectDTO> AddAsync(SubjectForCreationDTO subject)
    {
        Subject sbjObj = _sm.Mapper.Map<Subject>(subject);
        foreach (string mail in subject.Teachers)
        {
            //User? choosenTeacher = _dbContext.Users.Where(user => user.Email == mail).FirstOrDefault();
            User? choosenTeacher = await _rm.UserRepository.GetUserByMailAsync(mail);
            if (choosenTeacher != null)
            {
                if ((await _userManager.GetRolesAsync(choosenTeacher))?.Contains("User") == true)
                    await _sm.TeacherService.AssignTeacherRoleToUser(choosenTeacher.Id);
                sbjObj.Teachers.Add(choosenTeacher);
            }
        }
        sbjObj.Topics = [];
        return _sm.Mapper.Map<SubjectDTO>(await _rm.SubjectRepository.AddAsync(sbjObj));
    }


    public async Task DeleteAsync(Guid id)
    {
        var subject = await _rm.SubjectRepository.GetByIdAsync(id);

        if (subject == null)
        {
            NotFound();
        }

        await _rm.SubjectRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<SubjectDTO>> GetAllAsync()
    {
        return _sm.Mapper.Map<IEnumerable<SubjectDTO>>(await _rm.SubjectRepository.GetAllAsync());
    }

    public async Task<IEnumerable<SubjectDTO>> GetTeacherSubjectsAsync()
    {
        var teacherId = _sm.TokenService.GetUserId();

        return _sm.Mapper.Map<IEnumerable<SubjectDTO>>(
            await _rm.SubjectRepository.GetTeachersSubjectsAsync(teacherId));
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
        var sbjObj = await _rm.SubjectRepository.GetByIdAsync(Id);

        if (sbjObj == null)
        {
            NotFound($"No answer with id {Id} exist.");
        }

        _sm.Mapper.Map(subject, sbjObj);
        sbjObj.Teachers.Clear();
        foreach (string mail in subject.Teachers)
        {
            User? choosenTeacher = await _rm.UserRepository.GetUserByMailAsync(mail);
            if (choosenTeacher != null)
            {
                if ((await _userManager.GetRolesAsync(choosenTeacher))?.Contains("User") == true)
                    await _sm.TeacherService.AssignTeacherRoleToUser(choosenTeacher.Id);
                sbjObj.Teachers.Add(choosenTeacher);
            }
        }

        await _rm.SubjectRepository.UpdateAsync(sbjObj);
    }
}
