using Domain.DTO.Response;
using Domain.Entities;

namespace TestUtility.Factories;

public static class TagFactory
{
    public static Tag CreateTag(
        Guid id,
        string value = "TestTag"
    ) => new()
    {
        Id = id,
        Value = value
    };

    public static Tag CreateTag(
        string value = "TestTag"
    ) => new()
    {
        Value = value
    };

    public static TagStandardDTO CreateTagStandardDTO(
        Guid? id,
        string value = "TestTag"
    ) => new()
    {
        Value = value
    };
}
