using infra.Context;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace integration;

public class IntegrationTest : IClassFixture<ApiFactory>, IAsyncLifetime
{
    protected readonly HttpClient HttpClient;
    protected readonly DataContext DataContext;

    protected IntegrationTest(ApiFactory apiFactory)
    {
        var scope = apiFactory.Services.CreateScope();

        HttpClient = apiFactory.CreateClient();
        DataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    }

    public async Task InitializeAsync()
    {
        await DataContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DataContext.Database.EnsureDeletedAsync();
    }
}