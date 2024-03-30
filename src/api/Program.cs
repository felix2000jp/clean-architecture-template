using api.Configuration;
using api.Middleware;
using api.Notes;
using infra;
using Serilog;
using service;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHeaderPropagation(options => options.Headers.Add(CustomHeaders.CorrelationId));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(typeof(Program));

    builder.Services.AddInfraLayer(builder.Configuration);
    builder.Services.AddServiceLayer();
}

var app = builder.Build();
{
    app.Services.ApplyMigrations();

    app.UseMiddleware<LoggerCorrelationMiddleware>();
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseHeaderPropagation();
    app.UseHttpsRedirection();

    app.UseSwagger();
    app.UseSwaggerUI(options => options.DefaultModelsExpandDepth(-1));

    app.MapNoteRoutes();
    app.Run();
}

public partial class Program;