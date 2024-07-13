using core.Notes;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using service.Behaviours;
using service.Notes;

namespace service;

public static class DependencyInjection
{
    public static void AddServiceLayer(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddScoped<INoteService, NoteService>();

        services.AddAutoMapper(typeof(DependencyInjection));
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });
    }
}