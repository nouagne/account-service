# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and all projects .csproj files (necessary for dotnet restore)
COPY TemplateProject.sln .

COPY src/TemplateProject.API/TemplateProject.API.csproj ./src/TemplateProject.API/
COPY src/TemplateProject.Application/TemplateProject.Application.csproj ./src/TemplateProject.Application/
COPY src/TemplateProject.Domain/TemplateProject.Domain.csproj ./src/TemplateProject.Domain/
COPY src/TemplateProject.Infrastructure/TemplateProject.Infrastructure.csproj ./src/TemplateProject.Infrastructure/

COPY tests/TemplateProject.UnitTests/TemplateProject.UnitTests.csproj ./tests/TemplateProject.UnitTests/

# Now that all referenced projects exist: restore
RUN dotnet restore

# Copy full source
COPY . .

# Build
WORKDIR /app/src/TemplateProject.API
RUN dotnet build -c Release -o /app/build

# Publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TemplateProject.API.dll"]
