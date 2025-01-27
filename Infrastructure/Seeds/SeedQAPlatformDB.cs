using Bogus;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds;

public static class SeedQAPlatformDB
{
    public static async Task RunAsync(
        QAPlatformContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager
    ) {
        var subjects = CreateSubjects();
        await context.AddRangeAsync(subjects);

        await CreateUserRoles(roleManager);

        var users = await CreateUsers(50, userManager);

        var topics = CreateTopics(subjects);
        await context.AddRangeAsync(topics);

        var tags = CreateTags();
        await context.AddRangeAsync(tags);

        var questions = CreateQuestions(10, topics, tags, users);
        await context.AddRangeAsync(questions);

        var answers = CreateAnswers(5, questions, users);
        await context.AddRangeAsync(answers);

        var comments = CreateComments(5, answers, users);
        await context.AddRangeAsync(comments);

        await context.SaveChangesAsync();
    }

    private static List<Subject> CreateSubjects()
    {
        var subjects = new HashSet<Subject>();

        var categories = new[]
        {
            "Mathematics",
            "Physics",
            "Biology",
            "Computer Science",
            "History",
            "Psychology",
            "Engineering",
            "Economics",
            "Art",
            "Law"
        };

        var levels = new[]
        {
            "Introduction to",
            "Advanced",
            "Fundamentals of",
            "Essentials of"
        };

        foreach (var category in categories)
        {
            foreach (var level in levels)
            {
                var subject = new Faker<Subject>().Rules((f, s) =>
                {
                    s.Name = $"{level} {category}";
                    s.SubjectCode = $"{f.Random.Int(100, 999)}-{f.Random.Int(100, 999)}-{f.Random.Int(100, 999)}";
                });
                subjects.Add(subject);
            }
        }

        var generalSubject = new Faker<Subject>().Rules((f, s) =>
        {
            s.Name = $"General";
            s.SubjectCode = $"{f.Random.Int(100, 999)}-{f.Random.Int(100, 999)}-{f.Random.Int(100, 999)}";
        });
        subjects.Add(generalSubject);

        return [.. subjects];
    }

    private static async Task CreateUserRoles(
        RoleManager<IdentityRole> roleManager
    ) {
        string[] roles = [Roles.TEACHER, Roles.USER, Roles.ADMIN];
        foreach (var roleName in roles)
        {
            if (await roleManager.RoleExistsAsync(roleName)) continue;
            var role = new IdentityRole { Name = roleName };
            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
        }
    }

    private static async Task<List<User>> CreateUsers(
        int nrOfUsers,
        UserManager<User> userManager,
        int nrOfAdmins = 1,
        string password = "password"
    ) {
        var faker = new Faker<User>("en").Rules((f, user) =>
        {
            user.Email = f.Person.Email;
            user.UserName = f.Person.UserName;
            user.IsBlocked = false;
        });

        var users = faker.Generate(nrOfUsers);

        foreach (var user in users.GetRange(0, nrOfAdmins))
        {
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            await userManager.AddToRoleAsync(user, Roles.USER);
            await userManager.AddToRoleAsync(user, Roles.TEACHER);
            await userManager.AddToRoleAsync(user, Roles.ADMIN);
        }

        foreach (var user in users.GetRange(nrOfAdmins, users.Count - nrOfAdmins))
        {
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            await userManager.AddToRoleAsync(user, Roles.USER);
        }

        return users;
    }

    private static List<Topic> CreateTopics(
        List<Subject> subjects
    ) {
        var topics = new List<Topic>();
        var topicsNames = new[]
        {
            "Assignment 1",
            "Assignment 2",
            "Assignment 3",
            "Lab 1",
            "Lab 2",
            "Lab 3",
            "Exam"
        };

        foreach (var subject in subjects)
        {
            foreach (var name in topicsNames)
            {
                var topic = new Topic()
                {
                    SubjectId = subject.Id,
                    Name = name,
                    IsActive = true
                };
                topics.Add(topic);
            }
        }

        return topics;
    }

    private static List<Tag> CreateTags()
    {
        var tags = new List<Tag>();

        var tagValues = new List<string>
        {
            "Admissions",
            "Scholarships",
            "Exams",
            "Assignments",
            "Research",
            "Courses",
            "StudentLife",
            "Housing",
            "Clubs",
            "Internships"
        };

        foreach(var tagName in tagValues)
        {
            var tag = new Tag()
            {
                Value = tagName
            };
            tags.Add(tag);
        }

        return tags;
    }

    private static List<Question> CreateQuestions(
        int maxQuantity,
        List<Topic> topics,
        List<Tag> tags,
        List<User> users
    ) {
        var questions = new List<Question>();

        foreach (var topic in topics)
        {
            var derivedQuantities = RandomInt(0, maxQuantity);
            for (int i = 0; i < derivedQuantities; i++)
            {
                var question = new Faker<Question>("en").Rules((f, q) =>
                {
                    q.TopicId = topic.Id;
                    q.UserId = users[RandomInt(0, users.Count)].Id;
                    q.Title = $"{f.Lorem.Sentence(5, 20)}?";
                    q.Description = f.Lorem.Sentence(20, 200);
                    q.Created = DateTime.SpecifyKind(f.Date.Between(DateTime.UtcNow, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)), DateTimeKind.Utc);
                    q.IsResolved = RandomBool();
                    q.IsProtected = RandomBool();
                    q.IsHidden = RandomBool();
                    q.Tags = tags.GetRange(0, RandomInt(1, tags.Count));
                });
                questions.Add(question);
            }
        }

        return questions;
    }

    private static List<Answer> CreateAnswers(
        int maxQuantity,
        List<Question> questions,
        List<User> users
    ) {
        var answers = new List<Answer>();

        foreach (var question in questions)
        {
            var derivedQuantities = RandomInt(0, maxQuantity);
            for (int i = 0; i < derivedQuantities; i++)
            {
                var answer = new Faker<Answer>("en").Rules((f, a) =>
                {
                    a.QuestionId = question.Id;
                    a.UserId = users[RandomInt(0, users.Count)].Id;
                    a.Value = $"{f.Lorem.Sentence(20, 100)}";
                    a.Rating = 0;
                    a.Created = DateTime.SpecifyKind(f.Date.Between(question.Created.AddDays(1), question.Created.AddDays(100)), DateTimeKind.Utc);
                    a.IsHidden = RandomBool();
                });
                answers.Add(answer);
            }
        }

        return answers;
    }

    private static List<Comment> CreateComments(
        int maxQuantity,
        List<Answer> answers,
        List<User> users
    ) {
        var comments = new List<Comment>();

        foreach (var answer in answers)
        {
            var derivedQuantities = RandomInt(0, maxQuantity);
            for (int i = 0; i < derivedQuantities; i++)
            {
                var comment = new Faker<Comment>("en").Rules((f, c) =>
                {
                    c.AnswerId = answer.Id;
                    c.UserId = users[RandomInt(0, users.Count)].Id;
                    c.Value = $"{f.Lorem.Sentence(20, 50)}";
                });
                comments.Add(comment);
            }
        }

        return comments;
    }

    private static bool RandomBool()
    {
        Random random = new Random();
        return random.Next(0, 2) == 0;
    }

    private static int RandomInt(int min, int max) {
        Random random = new Random();
        int number = random.Next(min, max);
        return number;
    }
}
