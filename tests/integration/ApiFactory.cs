using infra.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
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

    private readonly RedisContainer _cacheContainer = new RedisBuilder()
        .WithImage("redis")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var databaseConnectionString = _databaseContainer.GetConnectionString();
        var cacheConnectionString = _cacheContainer.GetConnectionString();

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<DataContext>));
            services.RemoveAll(typeof(IDistributedCache));
            
            services.AddDbContext<DataContext>(options => options.UseNpgsql(databaseConnectionString));
            services.AddStackExchangeRedisCache(options => options.Configuration = cacheConnectionString);
        });
    }

    public async Task InitializeAsync()
    {
        await _databaseContainer.StartAsync();
        await _cacheContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _databaseContainer.DisposeAsync();
        await _cacheContainer.DisposeAsync();
    }
}