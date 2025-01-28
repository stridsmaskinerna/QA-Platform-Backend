using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO;

public class CommentDTO
{
    public Guid Id { get; set; }

    public string? UserId { get; set; }
    public string? UserName { get; set; }

    public string Value { get; set; } = string.Empty;
}
