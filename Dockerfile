# ========= Restore (cache) =========
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src
COPY AccountService.sln ./
COPY src/AccountService.API/AccountService.API.csproj src/AccountService.API/
COPY src/AccountService.Application/AccountService.Application.csproj src/AccountService.Application/
COPY src/AccountService.Domain/AccountService.Domain.csproj src/AccountService.Domain/
COPY src/AccountService.Infrastructure/AccountService.Infrastructure.csproj src/AccountService.Infrastructure/
RUN dotnet restore AccountService.sln

# ========= Build/Publish =========
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
# Debug rapide si besoin :
# RUN ls -la && ls -la src && ls -la src/AccountService.API
RUN dotnet publish src/AccountService.API/AccountService.API.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ========= Runtime =========
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "AccountService.API.dll"]
