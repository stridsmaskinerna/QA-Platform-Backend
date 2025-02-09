using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Response;

public class TopicDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive {  get; set; }
    public Guid SubjectId { get; set; }

}
