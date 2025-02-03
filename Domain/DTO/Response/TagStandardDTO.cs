using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Response;

public class TagStandardDTO
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
}
