namespace Infrastructure.Entities;

public class Comment
{
    public Guid Id { get; set; }

    public string Value { get; set; } = String.Empty;
}
