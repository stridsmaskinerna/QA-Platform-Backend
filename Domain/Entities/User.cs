using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser
{
    public bool IsBlocked { get; set; }

    //Navigation

    public ICollection<Question> Questions { get; set; } = [];

    public ICollection<Answer> Answers { get; set; } = [];

    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<Subject> Subjects { get; set; } = [];
}
