# QA-Platform-Backend

## Design

- [Database ER diagram](https://lucid.app/lucidchart/ae37e314-f5bd-427d-92c3-6db17f0c7d96/edit?viewport_loc=39%2C-134%2C2908%2C1554%2C0_0&invitationId=inv_407d2aa7-e46e-4486-bd13-9dc5f04f6818)

- [Sequence diagrams](https://lucid.app/lucidchart/40271668-76b3-4095-9ac9-0968643d98d7/edit?invitationId=inv_1247fee7-1320-4446-be3a-d6849eb6419e&page=0_0#)

- [Package diagram](https://lucid.app/lucidchart/81d55cd4-06ae-4ec0-867b-00a1f21cb27e/edit?invitationId=inv_24927175-c561-46cd-b5c6-94fcadcecb3d&page=0_0#)

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
