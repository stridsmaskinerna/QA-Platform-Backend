# QA-Platform-Backend

## External Doc

- **[Recommended Deployment Strategy for QA Platform](Doc/DEPLOYMENT.md)** 🔗

- **[Database ER diagram](https://lucid.app/lucidchart/ae37e314-f5bd-427d-92c3-6db17f0c7d96/edit?viewport_loc=39%2C-134%2C2908%2C1554%2C0_0&invitationId=inv_407d2aa7-e46e-4486-bd13-9dc5f04f6818) 🔗**

- **[Sequence diagrams](https://lucid.app/lucidchart/40271668-76b3-4095-9ac9-0968643d98d7/edit?invitationId=inv_1247fee7-1320-4446-be3a-d6849eb6419e&page=0_0#) 🔗**

- **[Package diagram](https://lucid.app/lucidchart/81d55cd4-06ae-4ec0-867b-00a1f21cb27e/edit?invitationId=inv_24927175-c561-46cd-b5c6-94fcadcecb3d&page=0_0#) 🔗**

## Content

- **[Development requirements](#development-requirements)**
- **[Docker](#docker)**
- **[Postgres ADMIN](#postgres-admin)**
- **[Formatting](#formatting)**
- **[Testing Strategy](#testing-strategy)**
  - [Unit Tests](#unit-tests)
  - [Integration Tests](#integration-tests)
  - [Acceptance Tests](#acceptance-tests)      

## Development requirements

- Dotnet version 8

- Docker version where you run `docker compose` **not** `docker-compose`

- Create a [.NET secret file](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0&tabs=windows) for project `QAPlatformAPI`:

```
{
  "secretKey": "Z7yCwJqQBrpqTEx9UmzXiedyzWSPF6cM"
}
```

## Docker

- **For Development (Postgres DB, Postgres ADMIN)**: `docker compose --file docker-compose-dev.yml up -d`

- **For Testing (Postgres DB, Postgres ADMIN)**: `docker compose --file docker-compose-test.yml up -d`

- **For Production/Staging (Postgres DB, Application)**: `docker compose --file docker-compose-prod.yml up -d`

## Postgres ADMIN

After stating docker for *development* or for *testing* you can log in to `Postgres ADMIN` at `localhost:8081`

- Username/email: `dev@dev.com`
- Password: `devpassword`

After login, you need to register the database server by completing the following steps:

- Click `Add new Server`
- In the `General` tab, enter: 
    - Name: `qa-platform-db`
    - Server group: `Servers`
- In the `Connection` tab, enter:
    - Host name/address: `qa-platform-db`
    - Port: 5432 
    - Username: `devuser`
    - Password: `devpassword`
- Click `Save`

After `Save`, the database can be found under `Serves`in the left board.

## Formatting

-   To change formatting rules, go to .editorconfig file in the root folder. To enforce a rule across the whole project, install dotnet format: `dotnet tool install -g dotnet-format` then run `dotnet format`. Be mindful of how this affect others in the project.
-   **Setup**: In vs code install the extension "EditorConfig for VS Code". For Visual Studio the setting “Enable EditorConfig support” must be turned on (which is the default).

## Testing Strategy

#### Unit Tests

Service classes are unit-tested by mocking all of the dependencies. For example, adding a tag in `TagService.cs`

```csharp
public class TagServiceTests : SetupServiceTests
{
    private readonly TagService _tagService;

    public TagServiceTests()
    {
        _tagService = new TagService(
            _mockRepositoryManager.Object,
            _mockServiceManager.Object);
    }

    public class AddAsync : TagServiceTests
    {
        [Fact]
        public async Task ShouldReturnTagDTO_WhenTagIsAdded()
        {
            // Arrange
            var tagEntity = TagFactory.CreateTag(Guid.NewGuid(), "TestTag");

            var tagDTO = TagFactory.CreateTagStandardDTO(tagEntity.Id, tagEntity.Value);

            _mockTagRepository
                .Setup(r => r.AddAsync(tagEntity))
                .ReturnsAsync(tagEntity);

            _mockMapper
                .Setup(m => m.Map<TagStandardDTO>(tagEntity))
                .Returns(tagDTO);

            // Act
            var result = await _tagService.AddAsync(tagEntity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tagDTO.Id, result.Id);
            Assert.Equal(tagDTO.Value, result.Value);
        }
    }
}
```

#### Integration Tests

The backend and the database are tested by using a test database. For example, testing user registration:

```csharp
public class AuthenticationControllerTests : IntegrationTestBase
{
    public AuthenticationControllerTests(QAPlatformAPIFactory<Program> factory) :
        base(factory)
    { }

    public class Register : AuthenticationControllerTests
    {
        private const string _endpoint = "/api/authentication/register";

        public Register(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [Theory]
        [InlineData("12newTestUser", "password")]
        [InlineData("123newTestUser", "PASSWORD")]
        [InlineData("1234newTestUser", "12345678")]
        [InlineData("12335newTestUser", "00000000")]
        public async Task ShouldReturn_Ok_WhenValidRegistrationData(
            string userName,
            string password
        )
        {
            // Arrange
            var requestBody = AuthenticationFactory.CreateRegistrationDTO(
                userName,
                $"{userName}@ltu.se",
                password
            );

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
```

#### Acceptance Tests

Acceptance tests are used to relate tests to *user stories*. Acceptance tests are
implemented by calling the relevant integration tests. 

For example, an unauthenticated user should be able to register and log in **(User Story: UNAUTH_USER.ESSE.2)**:

```csharp
public class UnauthenticatedUser : UserStoryTestBase
{
    public UnauthenticatedUser(QAPlatformAPIFactory<Program> factory) :
        base(factory)
    { }

    public class Essential : UnauthenticatedUser
    {
        public Essential(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [UserStory(Description.UNAUTH_USER_ESSE_2)]
        public async Task UNAUTH_USER_ESSE_2()
        {
            await RunTestsAsync<AuthenticationControllerTests.Register>();
            await RunTestsAsync<AuthenticationControllerTests.Login>();
        }
    }
}
```
