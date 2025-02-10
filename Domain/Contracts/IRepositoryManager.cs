using Infrastructure.Repositories;

namespace Domain.Contracts;

public interface IRepositoryManager
{
    IAnswerRepository AnswerRepository { get; }
    IAnswerVoteRepository AnswerVoteRepository { get; }
    ICommentRepository CommentRepository { get; }
    IQuestionRepository QuestionRepository { get; }
    ISubjectRepository SubjectRepository { get; }
    ITagRepository TagRepository { get; }
    ITopicRepository TopicRepository { get; }
    IUserRepository UserRepository { get; }
}
