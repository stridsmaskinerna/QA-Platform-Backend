﻿using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public record AnswerDTO
    {
        public Guid Id;
        public Guid QuestionId;
        public string UserName;
        public string Value;
        public int Rating;
        public string? FilePath;
        public DateTime Created;
        public bool IsHidden;
        public IEnumerable<CommentDTO> Comments;
    }
}
