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
RUN dotnet build "api/api.csproj"

FROM build AS tests-unit
WORKDIR /tests
COPY ["tests/unit/unit.csproj", "unit/"]
RUN dotnet restore "unit/unit.csproj"
COPY ["tests/unit/", "unit/"]
ENTRYPOINT ["dotnet", "test", "unit/unit.csproj"]

FROM tests-unit AS tests-unit-runner
WORKDIR /tests
ENTRYPOINT ["dotnet", "test", "unit/unit.csproj"]

FROM build AS tests-integration
WORKDIR /tests
COPY ["tests/integration/integration.csproj", "integration/"]
RUN dotnet restore "integration/integration.csproj"
COPY ["tests/integration/", "integration/"]
ENTRYPOINT ["dotnet", "test", "integration/integration.csproj"]

FROM build AS tests-integration-runner
WORKDIR /tests
ENTRYPOINT ["dotnet", "test", "integration/integration.csproj"]

FROM build AS publish
RUN dotnet publish "api/api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api.dll"]
