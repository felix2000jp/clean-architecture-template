using core.Settings;
using infra.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace integration;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _databaseContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("notes-db")
        .WithUsername("root")
        .WithPassword("1234")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Configure<PersistenceSettings>(options =>
            {
                options.DatabaseConnection = _databaseContainer.GetConnectionString();
            });
        });
    }

    public async Task InitializeAsync() => await _databaseContainer.StartAsync();

    public new async Task DisposeAsync() => await _databaseContainer.DisposeAsync().AsTask();
}