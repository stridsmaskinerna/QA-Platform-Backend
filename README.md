# QA-Platform-Backend

## Development requirements

- Dotnet version 8

- Docker version where you run `docker compose` **not** `docker-compose`

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

After `Save`, the database can be found under `Serves`int the left board.
 

## Formatting

-   To change formatting rules, go to .editorconfig file in the root folder. To enforce a rule across the whole project, install dotnet format: `dotnet tool install -g dotnet-format` then run `dotnet format`. Be mindful of how this affect others in the project.
-   **Setup**: In vs code install the extension "EditorConfig for VS Code". For Visual Studio the setting “Enable EditorConfig support” must be turned on (which is the default).
