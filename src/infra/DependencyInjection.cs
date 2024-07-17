using core;
using core.Notes;
using core.Settings;
using infra.Context;
using infra.Notes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace infra;

public static class DependencyInjection
{
    public static void AddInfraLayer(this IServiceCollection services)
    {
        services.AddDbContext<DataContext>((serviceProvider, options) =>
        {
            var persistenceSettings = serviceProvider.GetRequiredService<IOptions<PersistenceSettings>>().Value;
            options.UseNpgsql(persistenceSettings.DatabaseConnection);
        });
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