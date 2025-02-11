using Domain.Contracts;

namespace Infrastructure.Repositories;

public class RepositoryManager(
    Lazy<IAnswerRepository> answerRepository,
    Lazy<IAnswerVoteRepository> answerVoteRepository,
    Lazy<ICommentRepository> commentRepository,
    Lazy<IQuestionRepository> questionRepository,
    Lazy<ISubjectRepository> subjectRepository,
    Lazy<ITagRepository> tagRepository,
    Lazy<ITopicRepository> topicRepository,
    Lazy<IUserRepository> userRepository
) : IRepositoryManager
{
    public IAnswerRepository AnswerRepository => answerRepository.Value;

    public IAnswerVoteRepository AnswerVoteRepository => answerVoteRepository.Value;

    public ICommentRepository CommentRepository => commentRepository.Value;

    public IQuestionRepository QuestionRepository => questionRepository.Value;

    public ISubjectRepository SubjectRepository => subjectRepository.Value;

    public ITagRepository TagRepository => tagRepository.Value;

    public ITopicRepository TopicRepository => topicRepository.Value;

    public IUserRepository UserRepository => userRepository.Value;
}
