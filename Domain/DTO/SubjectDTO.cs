using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public record SubjectDTO
    {
        public Guid Id;
        public string Name;
        public string? SubjectCode;
        public IEnumerable<UserWithEmailDTO> Theachers;
    }
}
