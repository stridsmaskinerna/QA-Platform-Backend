using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser
{
    public bool IsBlocked { get; set; }

    //Navigation

    public IEnumerable<Question> Questions { get; set; } = [];

    public IEnumerable<Answer> Answers { get; set; } = [];

    public IEnumerable<Comment> Comments { get; set; } = [];

    public IEnumerable<Subject> Subjects { get; set; } = [];
}
