using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO;

public class SubjectDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SubjectCode { get; set; }
    public IEnumerable<UserWithEmailDTO> Theachers { get; set; } = [];
}
