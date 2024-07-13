FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /src
COPY ["src/api/", "api/"]
COPY ["src/service/", "service/"]
COPY ["src/core/", "core/"]
COPY ["src/infra/", "infra/"]
RUN dotnet build "api/api.csproj"


FROM builder AS tests-unit
WORKDIR /tests
COPY ["tests/unit/", "unit/"]
RUN dotnet build "unit/unit.csproj"
ENTRYPOINT ["dotnet", "test", "unit/unit.csproj", "--no-restore", "--no-build"]


FROM builder AS tests-integration
WORKDIR /tests
COPY ["tests/integration/", "integration/"]
RUN dotnet build "integration/integration.csproj"
ENTRYPOINT ["dotnet", "test", "integration/integration.csproj", "--no-restore", "--no-build"]


FROM builder AS publish
WORKDIR /src
RUN dotnet publish "api/api.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api.dll"]
