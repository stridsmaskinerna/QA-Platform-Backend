using System.Text;
using System.Text.Json;
using Application.ProfilesMaps;
using Application.Services;
using Domain.Constants;
using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace QAPlatformAPI.Extensions;

public static class WebApplicationBuilderExtension
{
    public static void AddDBExtension(this WebApplicationBuilder builder)
    {

        builder.Services.AddDbContext<QAPlatformContext>(options =>
        {
            var connectionString =
                builder.Configuration.GetConnectionString("PostgreSQLConnection")
                ?? throw new InvalidOperationException("Connection string not found.");

            options.UseNpgsql(connectionString);

            if (builder.Environment.IsDevelopment())
            {
                Console.WriteLine($"Connection String: {connectionString}");
            }

            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
           .AddEntityFrameworkStores<QAPlatformContext>()
           .AddDefaultTokenProviders();
    }

    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<AnswerProfileMapper>();
                                                cfg.AddProfile<CommentProfileMapper>();
                                                cfg.AddProfile<SubjectProfileMapper>();
                                                cfg.AddProfile<UserProfileMapper>();
                                                cfg.AddProfile<QuestionProfileMapper>(); });

        builder.Services.AddScoped<IServiceManager, ServiceManager>();
        builder.Services.AddAsLazy<IBaseService, BaseService>();
        builder.Services.AddAsLazy<IQuestionService, QuestionService>();
        builder.Services.AddAsLazy<IAnswerService, AnswerService>();
        builder.Services.AddAsLazy<IAuthenticationService, AuthenticationService>();
        builder.Services.AddAsLazy<ITokenService, TokenService>();

        builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
        builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();
        builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
        builder.Services.AddScoped<ITopicRepository, TopicRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddAsLazy<IServiceType, ServiceType>(
        this IServiceCollection collection,
        ServiceLifetime lifetime = ServiceLifetime.Scoped
    )
        where ServiceType : class, IServiceType
        where IServiceType : class
    {
        collection.Add(new ServiceDescriptor(
            typeof(IServiceType),
            typeof(ServiceType),
            lifetime
        ));

        collection.Add(new ServiceDescriptor(
            typeof(Lazy<IServiceType>),
            p => new Lazy<IServiceType>(() => p.GetRequiredService<IServiceType>()),
            lifetime
        ));
    }

    public static void AddJSONSerializerOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }

    public static void AddIdentityCoreExtension(this WebApplicationBuilder builder)
    {
        builder.Services.AddDataProtection();

        builder.Services
            .AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration?["secretKey"]
                        ?? throw new InvalidOperationException("secretKey string not found."))
                    ),
                }
            );

        builder.Services.AddIdentityCore<User>(opt =>
        {
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredLength = 8;

        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<QAPlatformContext>()
            .AddDefaultTokenProviders();
    }

    public static void AddCORSConfiguration(this WebApplicationBuilder builder)
    {
        var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        builder.Services.AddCors(config =>
            config.AddPolicy("AllowFrontend",
                p => p.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader() // ✅ Allows `Authorization` header
                      .WithExposedHeaders([
                          CustomHeaders.Pagination
                      ])
            ));
    }
}
