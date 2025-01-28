using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;


namespace Domain.DTO;

public class QuestionDTO
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public string? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public DateTime Created { get; set; }
    public bool IsResolved { get; set; }
    public bool IsProtected { get; set; }
    public bool IsHidden { get; set; }
    public ICollection<Tag>? Tags { get; set; }
}

public class QuestionDetailedDTO : QuestionDTO
{
    public ICollection<AnswerDTO>? Answers { get; set; }

}


