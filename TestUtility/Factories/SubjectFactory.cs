using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace TestUtility.Factories;

public static class SubjectFactory
{
    public static SubjectForCreationDTO CreateSubjectForCreationDTO(
        string name,
        string subjectCode
    ) => new()
    {
        Name = name,
        SubjectCode = subjectCode
    };

    public static SubjectForCreationDTO CreateSubjectForCreationDTO(
        string name,
        string subjectCode,
        ICollection<string> teachers
    ) => new()
    {
        Name = name,
        SubjectCode = subjectCode,
        Teachers = teachers
    };

    public static SubjectDTO CreateSubjectDTO(
        Guid id,
        string name,
        string subjectCode,
        IEnumerable<UserWithEmailDTO> teachers
    ) => new()
    {
        Id = id,
        Name = name,
        SubjectCode = subjectCode,
        Teachers = teachers
    };

    public static Subject CreateSubjectEntity(
        Guid id,
        string name,
        string subjectCode
    ) => new()
    {
        Id = id,
        Name = name,
        SubjectCode = subjectCode
    };
}
