FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /src
COPY ["src/api/", "api/"]
COPY ["src/service/", "service/"]
COPY ["src/core/", "core/"]
COPY ["src/infra/", "infra/"]
WORKDIR /tests
COPY ["tests/unit/", "unit/"]
COPY ["tests/integration/", "integration/"]
WORKDIR /
COPY ["clean-architecture-template.sln", "."]
RUN dotnet tool install JetBrains.ReSharper.GlobalTools --global
RUN /root/.dotnet/tools/jb inspectcode clean-architecture-template.sln --build --output=inspectcode.xml --verbosity=WARN --severity=WARNING && if grep "<Issue TypeId" inspectcode.xml; then echo "Fix the above warnings" && exit 1; fi
RUN dotnet build "src/api/api.csproj"


FROM builder AS tests-unit
WORKDIR /tests
RUN dotnet build "unit/unit.csproj"
ENTRYPOINT ["dotnet", "test", "unit/unit.csproj", "--no-restore", "--no-build"]


FROM builder AS tests-integration
WORKDIR /tests
RUN dotnet build "integration/integration.csproj"
ENTRYPOINT ["dotnet", "test", "integration/integration.csproj", "--no-restore", "--no-build"]


FROM builder AS publish
WORKDIR /src
RUN dotnet publish "api/api.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api.dll"]
