using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class QuestionDTO
    {
        public Guid Id;

        public string Title;

        public string Description;

        public bool IsResolved;

        public DateTime Created;

        public UserDTO Author;

        public IEnumerable<string>? Tags;

        public string Topic;

        public string Subject;

        public int AnswerCount;
    }

    public class QuestionDetailedDTO : QuestionDTO
    {

        public IEnumerable<AnswerDTO>? Answers;

    }

}
