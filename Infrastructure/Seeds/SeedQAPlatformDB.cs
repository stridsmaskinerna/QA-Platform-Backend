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
    )
    {
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

        var answerVotes = CreateAnswerVotes(users, answers);
        await context.AddRangeAsync(answerVotes);

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

        string[] prefixes =
[
    "CS", "MA", "EN", "LA", "IT", "FR", "DE", "SP", "RU", "JP",
    "BR", "CH", "IN", "US", "UK"
];

        foreach (var category in categories)
        {
            foreach (var level in levels)
            {
                var subject = new Faker<Subject>().Rules((f, s) =>
                {
                    s.Name = $"{level} {category}";
                    s.SubjectCode = $"{prefixes[RandomInt(0, prefixes.Length)]}{f.Random.Int(100, 999)}";
                });
                subjects.Add(subject);
            }
        }

        var generalSubject = new Faker<Subject>().Rules((f, s) =>
        {
            s.Name = $"General";
            s.SubjectCode = null;
        });
        subjects.Add(generalSubject);

        return [.. subjects];
    }

    private static async Task CreateUserRoles(
        RoleManager<IdentityRole> roleManager
    )
    {
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
    )
    {
        var faker = new Faker<User>()
            .RuleFor(u => u.UserName, f => f.Person.UserName)
            .RuleFor(u => u.Email, (f, u) => $"{u.UserName}@ltu.se".ToLower());

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
    )
    {
        var topics = new List<Topic>();
        var topicsNames = new[]
        {
            "Introduction",
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
                    Subject = subject,
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

        foreach (var tagName in tagValues)
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
    )
    {
        var questions = new List<Question>();

        foreach (var topic in topics)
        {
            var derivedQuantities = RandomInt(0, maxQuantity);
            for (int i = 0; i < derivedQuantities; i++)
            {
                //To make questions not have that many tags
                var rndTags = new HashSet<Tag>();
                for (int j = 0; j < 5; j++)
                {
                    rndTags.Add(tags[RandomInt(0, tags.Count)]);
                }


                var idx = RandomInt(0, Dummydata.StudentQuestions.Length);
                var question = new Faker<Question>("en").Rules((f, q) =>
                {
                    q.TopicId = topic.Id;
                    q.UserId = users[RandomInt(0, users.Count)].Id;
                    q.Title = Dummydata.StudentQuestions[idx].Title;
                    q.Description = Dummydata.StudentQuestions[idx].Description;
                    q.Created = DateTime.SpecifyKind(f.Date.Between(DateTime.UtcNow, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)), DateTimeKind.Utc);
                    q.IsResolved = RandomBool();
                    q.IsProtected = RandomBool();
                    q.IsHidden = RandomInt(0, 11) > 8;
                    q.Tags = rndTags;
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
    )
    {
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
                    a.Created = DateTime.SpecifyKind(f.Date.Between(question.Created.AddDays(1), question.Created.AddDays(100)), DateTimeKind.Utc);
                    a.IsHidden = RandomInt(0, 11) > 8;
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
    )
    {
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

    private static List<AnswerVotes> CreateAnswerVotes(List<User> users, List<Answer> answers)
    {
        var answerVotes = new List<AnswerVotes>();

        foreach (var user in users)
        {

            //Simulate not all users have voted for every answer
            if (RandomInt(0, 11) > 8)
            {
                continue;
            }


            foreach (var answer in answers)
            {

                //Simulate not all users have voted for every answer
                if (RandomInt(0, 11) > 8)
                {
                    answerVotes.Add(new AnswerVotes() { Vote = MostlyTrueBool(), User = user, UserId = user.Id, Answer = answer, AnswerId = answer.Id });
                }


            }


        }
        return answerVotes;

    }



    private static bool RandomBool()
    {
        Random random = new Random();
        return random.Next(0, 2) == 0;
    }

    private static int RandomInt(int min, int max)
    {
        Random random = new Random();
        int number = random.Next(min, max);
        return number;
    }

    private static bool MostlyTrueBool()
    {
        Random random = new Random();
        return random.Next(0, 11) > 2;
    }




}


