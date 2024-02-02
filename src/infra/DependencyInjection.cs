using core;
using core.Notes;
using infra.Context;
using infra.Notes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace infra;

public static class DependencyInjection
{
    public static void AddInfraLayer(this IServiceCollection services, IConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        services.AddDbContext<DataContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void ApplyMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<DataContext>();

        if (context is null)
        {
            throw new ArgumentException("Data context cannot be null");
        }

        context.Database.Migrate();
    }
}