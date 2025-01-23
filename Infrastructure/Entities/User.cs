using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Entities;

public class User : IdentityUser
{
    public bool IsBlocked { get; set; }

    //Navigation

    public IEnumerable<Question> Questions { get; set; } = [];

    public IEnumerable<Answer> Answers { get; set; } = [];

    public IEnumerable<Comment> Comments { get; set; } = [];
}
