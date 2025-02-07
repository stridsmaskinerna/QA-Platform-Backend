using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Application.Contracts
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDTO>> GetAllAsync();
        Task<SubjectDTO> GetByIdAsync(Guid id);
        Task<SubjectDTO> GetByNameAsync(string name);
        Task<SubjectDTO> GetByCodeAsync(string code);
        Task<SubjectDTO> AddAsync(SubjectForCreationDTO subject);
        Task UpdateAsync(Guid Id, SubjectForCreationDTO subject);
        Task DeleteAsync(Guid id);
    }
}
