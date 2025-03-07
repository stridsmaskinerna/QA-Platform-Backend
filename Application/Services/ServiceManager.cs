using Application.Contracts;
using AutoMapper;

namespace Application.Services;

public class ServiceManager(
    Lazy<IBaseService> baseService,
    Lazy<IQuestionService> questionService,
    Lazy<IAnswerService> answerService,
    Lazy<ICommentService> commentService,
    Lazy<IAuthenticationService> authenticationService,
    Lazy<ITokenService> tokenService,
    Lazy<ITagService> tagService,
    Lazy<IUtilityService> utilityService,
    Lazy<IVoteService> voteService,
    Lazy<ITopicService> topicService,
    Lazy<ISubjectService> subjectManager,
    Lazy<IDTOService> dtoService,
    Lazy<ITeacherService> teacherService,
    Lazy<IAdminService> adminService,
    IMapper mapper
) : IServiceManager
{
    public IBaseService BaseService => baseService.Value;
    public IQuestionService QuestionService => questionService.Value;
    public IAuthenticationService AuthenticationService => authenticationService.Value;
    public ITokenService TokenService => tokenService.Value;
    public IMapper Mapper => mapper;
    public IAnswerService AnswerService => answerService.Value;
    public ITagService TagService => tagService.Value;
    public IUtilityService UtilityService => utilityService.Value;
    public ICommentService CommentService => commentService.Value;
    public IVoteService VoteService => voteService.Value;
    public ITopicService TopicService => topicService.Value;
    public ISubjectService SubjectService => subjectManager.Value;
    public ITeacherService TeacherService => teacherService.Value;
    public IAdminService AdminService => adminService.Value;
    public IDTOService DTOService => dtoService.Value;

}
