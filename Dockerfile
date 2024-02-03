FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/api/api.csproj", "api/"]
COPY ["src/service/service.csproj", "service/"]
COPY ["src/core/core.csproj", "core/"]
COPY ["src/infra/infra.csproj", "infra/"]
RUN dotnet restore "api/api.csproj"
COPY ["src/api/", "api/"]
COPY ["src/service/", "service/"]
COPY ["src/core/", "core/"]
COPY ["src/infra/", "infra/"]
RUN dotnet build "api/api.csproj" --no-restore

FROM build AS tests
WORKDIR /tests
COPY ["tests/unit/unit.csproj", "unit/"]
COPY ["tests/integration/integration.csproj", "integration/"]
RUN dotnet restore "unit/unit.csproj"
RUN dotnet restore "integration/integration.csproj"
COPY ["tests/unit/", "unit/"]
COPY ["tests/integration/", "integration/"]
RUN dotnet build "unit/unit.csproj" --no-restore
RUN dotnet build "integration/integration.csproj" --no-restore

FROM build AS tests-unit
WORKDIR /tests
ENTRYPOINT ["dotnet", "test", "unit/unit.csproj", "--no-restore", "--no-build"]

FROM build AS tests-integration
WORKDIR /tests
ENTRYPOINT ["dotnet", "test", "integration/integration.csproj", "--no-restore", "--no-build"]

FROM build AS publish
RUN dotnet publish "api/api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api.dll"]
