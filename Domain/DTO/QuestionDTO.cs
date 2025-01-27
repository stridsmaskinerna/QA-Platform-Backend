using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public record QuestionDTO
    {
        public Guid Id;

        public string? UserId;

        public string Title;

        public string Description;

        public string? FilePath;

        public DateTime Created;

        public bool IsResolved;

        public bool IsProtected;
    }

    
}
