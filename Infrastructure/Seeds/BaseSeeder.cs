using Bogus;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Seeds.Prod;
using Infrastructure.Seeds.Test;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds;

public class BaseSeeder : IBaseSeeder
{
    public List<Subject> CreateSubjects()
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
            "CS",
            "MA",
            "EN",
            "LA",
            "IT",
            "FR",
            "DE",
            "SP",
            "RU",
            "JP",
            "BR",
            "CH",
            "IN",
            "US",
            "UK"
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
            s.Name = SeedData.general;
            s.SubjectCode = null;
        });
        subjects.Add(generalSubject);

        return [.. subjects];
    }

    public async Task CreateUserRoles(
        RoleManager<IdentityRole> roleManager
    )
    {
        string[] roles =
        [
            DomainRoles.TEACHER,
            DomainRoles.USER,
            DomainRoles.ADMIN
        ];

        foreach (var roleName in roles)
        {
            if (await roleManager.RoleExistsAsync(roleName)) continue;
            var role = new IdentityRole { Name = roleName };
            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded) throw new SeedException(string.Join("\n", result.Errors));
        }
    }

    public async Task<List<User>> CreateUsers(
        int nrOfUsers,
        UserManager<User> userManager,
        List<Subject> subjects,
        int nrOfAdmins = 1,
        int nrOfTeachers = 10,
        string password = SeedDataTest.DEFAULT_PWD
    )
    {
        if (nrOfUsers < nrOfAdmins + nrOfTeachers)
        {
            throw new SeedException($"""
                "Nr of users '{nrOfTeachers}'
                can not be less then the sum of teachers and admins
                '{nrOfTeachers} + {nrOfAdmins}'.
                """);
        }

        var userSet = new HashSet<string>();

        var faker = new Faker<User>()
            .CustomInstantiator(f =>
            {
                string uniqueUserName;
                do
                {
                    uniqueUserName = f.Internet.UserName();
                } while (!userSet.Add(uniqueUserName));

                return new User
                {
                    UserName = uniqueUserName,
                    Email = $"{uniqueUserName}@ltu.se".ToLower()
                };
            });

        var users = faker.Generate(nrOfUsers + nrOfAdmins + nrOfTeachers);

        for (int i = 0; i < nrOfAdmins; i++)
        {
            var user = users[i];
            user.UserName = SeedDataTest.ADMIN_USERNAME;
            user.Email = SeedDataTest.ADMIN_EMAIL;

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new SeedException(string.Join("\n", result.Errors));

            await userManager.AddToRoleAsync(user, DomainRoles.TEACHER);
            await userManager.AddToRoleAsync(user, DomainRoles.USER);
            await userManager.AddToRoleAsync(user, DomainRoles.ADMIN);
        }

        for (int i = nrOfAdmins; i < nrOfAdmins + nrOfTeachers; i++)
        {
            var user = users[i];
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new SeedException(string.Join("\n", result.Errors));

            await userManager.AddToRoleAsync(user, DomainRoles.USER);
            await userManager.AddToRoleAsync(user, DomainRoles.TEACHER);

            for (int j = 0; j < subjects.Count; j++)
            {
                if (subjects[j].Name == SeedData.general)
                {
                    continue;
                }

                if (RandomInt(0, 11) > 8)
                {
                    user.Subjects.Add(subjects[j]);
                }
            }
        }

        for (int i = nrOfAdmins + nrOfTeachers; i < users.Count; i++)
        {
            var user = users[i];
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new SeedException(string.Join("\n", result.Errors));

            await userManager.AddToRoleAsync(user, DomainRoles.USER);
        }

        return users;
    }

    public List<Topic> CreateTopics(
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

    public List<Tag> CreateTags()
    {
        var faker = new Faker();
        var generatedTags = new HashSet<string>();

        var singleWords = new[]
        {
            "Admissions", "Scholarships", "Exams", "Assignments", "Research", "Courses",
            "StudentLife", "Housing", "Clubs", "Internships", "Education", "Technology",
            "Finance", "Marketing", "Engineering", "Mathematics", "Leadership",
            "Innovation", "Programming", "Entrepreneurship", "Science", "Biology", "Physics",
            "Chemistry", "Healthcare", "Economics", "Psychology", "Networking", "Security",
            "Database", "Statistics"
        };

        var firstWords = new[]
        {
            "Data", "Cloud", "Business", "Artificial", "Cyber", "Web", "Digital",
            "Software", "User", "Social", "Machine", "Blockchain", "Deep", "Virtual"
        };

        var secondWords = new[]
        {
            "Security", "Marketing", "Learning", "Development", "Automation",
            "Strategy", "Trends", "Intelligence", "Research", "Computing", "Analysis"
        };

        foreach (var word in singleWords)
        {
            generatedTags.Add(word.ToUpperInvariant());
        }

        foreach (var word1 in firstWords)
        {
            foreach (var word2 in secondWords)
            {
                generatedTags.Add($"{word1} {word2}".ToUpperInvariant());
            }
        }

        var random = new Random();

        return generatedTags
            .OrderBy(_ => random.Next())
            .Select(tag => new Tag { Value = tag })
            .ToList();
    }

    public List<Question> CreateQuestions(
        int maxQuantityPerTopic,
        List<Topic> topics,
        List<Tag> tags,
        List<User> users
    )
    {
        var questions = new List<Question>();

        foreach (var topic in topics)
        {
            var derivedQuantities = RandomInt(0, maxQuantityPerTopic);
            for (int i = 0; i < derivedQuantities; i++)
            {
                //To make questions not have that many tags
                var rndTags = new HashSet<Tag>();
                var nrOfTags = RandomInt(0, 8);

                for (int j = 0; j < nrOfTags; j++)
                {
                    rndTags.Add(tags[RandomInt(0, tags.Count)]);
                }

                var idx = RandomInt(0, SeedData.StudentQuestions.Length);
                var question = new Faker<Question>("en").Rules((f, q) =>
                {
                    q.TopicId = topic.Id;
                    q.UserId = users[RandomInt(0, users.Count)].Id;
                    q.Title = SeedData.StudentQuestions[idx].Title;
                    q.Description = SeedData.StudentQuestions[idx].Description;
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

    public List<Answer> CreateAnswers(
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
                    a.UserId = RandomInt(0, 11) > 3 ? users[RandomInt(0, users.Count)].Id
                    : users.Where(u => u.Subjects.Any(s => s.Id == question.Topic.SubjectId))
                    .Select(u => u.Id)
                    .FirstOrDefault();

                    a.Value = $"{f.Lorem.Sentence(20, 100)}";
                    a.Created = DateTime.SpecifyKind(f.Date.Between(question.Created.AddDays(1), question.Created.AddDays(100)), DateTimeKind.Utc);
                    a.IsHidden = RandomInt(0, 11) > 8;
                });
                answers.Add(answer);
            }
        }

        return answers;
    }

    public List<Comment> CreateComments(
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

    public List<AnswerVotes> CreateAnswerVotes(
        List<User> users,
        List<Answer> answers
    )
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
        var random = new Random();
        return random.Next(0, 2) == 0;
    }

    private static int RandomInt(int min, int max)
    {
        var random = new Random();
        int number = random.Next(min, max);
        return number;
    }

    private static bool MostlyTrueBool()
    {
        var random = new Random();
        return random.Next(0, 11) > 2;
    }
}
