using api.Configurations;
using api.Middlewares;
using api.Notes;
using core.Settings;
using infra;
using Serilog;
using service;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

    builder.Services
        .AddOptionsWithValidateOnStart<PersistenceSettings>()
        .BindConfiguration(PersistenceSettings.Section)
        .ValidateDataAnnotations();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHeaderPropagation(options => options.Headers.Add(CustomHeaders.CorrelationId));
    builder.Services.AddAutoMapper(typeof(Program));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddInfraLayer();
    builder.Services.AddServiceLayer();
}

var app = builder.Build();
{
    app.Services.ApplyMigrations();

    app.UseHttpsRedirection();
    app.UseMiddleware<LoggerCorrelationMiddleware>();
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseHeaderPropagation();

    app.UseSwagger();
    app.UseSwaggerUI(options => options.DefaultModelsExpandDepth(-1));

    app.MapNoteRoutes();
    app.Run();
}

public partial class Program;