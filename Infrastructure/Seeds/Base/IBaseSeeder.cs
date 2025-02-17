using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds.Base
{
    public interface IBaseSeeder
    {
        List<Answer> CreateAnswers(int maxQuantity, List<Question> questions, List<User> users);
        List<AnswerVotes> CreateAnswerVotes(List<User> users, List<Answer> answers);
        List<Comment> CreateComments(int maxQuantity, List<Answer> answers, List<User> users);
        List<Question> CreateQuestions(int maxQuantityPerTopic, List<Topic> topics, List<Tag> tags, List<User> users);
        List<Subject> CreateSubjects();
        List<Tag> CreateTags();
        List<Topic> CreateTopics(List<Subject> subjects);
        Task CreateUserRoles(RoleManager<IdentityRole> roleManager);
        Task<List<User>> CreateUsers(int nrOfUsers, UserManager<User> userManager, List<Subject> subjects, int nrOfAdmins = 1, int nrOfTeachers = 10, string password = "password");
    }
}
