using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.DTO;

public class AnswerDTO
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? FilePath { get; set; }
    public DateTime Created { get; set; }
    public IEnumerable<CommentDTO>? Comments { get; set; }
}
