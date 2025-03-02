# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files
COPY ["QAPlatformAPI/QAPlatformAPI.csproj", "QAPlatformAPI/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Presentation/Presentation.csproj", "Presentation/"]

# Restore dependencies
RUN dotnet restore "./QAPlatformAPI/QAPlatformAPI.csproj"

# Copy everything and build
COPY . .
WORKDIR /src/QAPlatformAPI
RUN dotnet build "./QAPlatformAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./QAPlatformAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy configuration files explicitly
# COPY ["QAPlatformAPI/appsettings.json", "appsettings.json"]
# COPY ["QAPlatformAPI/appsettings.Production.json", "appsettings.Production.json"]

# Ensure the environment is set to Production
ENV DOTNET_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "QAPlatformAPI.dll"]
