using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.DTO.Request;

public class SubjectForCreationDTO
{
    public string Name { get; set; } = string.Empty;
    public string? SubjectCode { get; set; }
    public ICollection<Guid> Teachers { get; set; } = [];
}
