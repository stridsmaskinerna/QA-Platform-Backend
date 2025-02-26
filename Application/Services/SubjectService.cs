using Application.Contracts;
using Domain.Constants;
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
            User? choosenTeacher = await _rm.UserRepository.GetUserByMailAsync(mail);
            if (choosenTeacher != null)
            {
                if ((await _userManager.GetRolesAsync(choosenTeacher))?.Contains(DomainRoles.TEACHER) == false)
                    await _sm.AdminService.AssignTeacherRoleToUser(choosenTeacher.Id);
                sbjObj.Teachers.Add(choosenTeacher);
            }
        }
        sbjObj.Topics =
        [
            new()
            {
                Name = $"{(sbjObj.SubjectCode != null ? sbjObj.SubjectCode + " " : "")}{sbjObj.Name}",
                Subject=sbjObj,
                IsActive=true
            }
        ];
        return _sm.Mapper.Map<SubjectDTO>(await _rm.SubjectRepository.AddAsync(sbjObj));
    }


    public async Task<Subject> DeleteAsync(Guid id)
    {
        var subject = await _rm.SubjectRepository.GetByIdAsync(id);

        if (subject == null)
        {
            NotFound();
        }

        var subjectDelete = await _rm.SubjectRepository.DeleteAsync(id);
        if (subjectDelete == null) Forbidden("Subeject cancellation not allowed: question are connected to the subject");
        return subjectDelete;
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

    public async Task<SubjectDTO> UpdateAsync(Guid Id, SubjectForCreationDTO subject)
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
                if ((await _userManager.GetRolesAsync(choosenTeacher))?.Contains(DomainRoles.TEACHER) == false)
                    await _sm.AdminService.AssignTeacherRoleToUser(choosenTeacher.Id);
                sbjObj.Teachers.Add(choosenTeacher);
            }
        }

        await _rm.SubjectRepository.UpdateAsync(sbjObj);

        return _sm.Mapper.Map<SubjectDTO>(sbjObj);
    }
}
