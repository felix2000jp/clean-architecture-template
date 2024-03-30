using api.Middleware;
using api.Notes;
using infra;
using Serilog;
using service;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(typeof(Program));

    builder.Services.AddInfraLayer(builder.Configuration);
    builder.Services.AddServiceLayer();
}

var app = builder.Build();
{
    app.Services.ApplyMigrations();

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();
    app.UseMiddleware<LoggerCorrelationMiddleware>();
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI(options => options.DefaultModelsExpandDepth(-1));

    app.MapNoteRoutes();
    app.Run();
}

public partial class Program;