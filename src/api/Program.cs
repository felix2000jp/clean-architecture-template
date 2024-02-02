using api.Middleware;
using api.Notes;
using infra;
using Serilog;
using service;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
    builder.Services.AddAutoMapper(typeof(Program));

    builder.Services.AddInfraLayer(builder.Configuration);
    builder.Services.AddServiceLayer();
}

var app = builder.Build();
{
    app.Services.ApplyMigrations();

    app.UseSerilogRequestLogging();
    app.UseMiddleware<ExceptionMiddleware>();

    app.InjectNoteRoutes();

    app.UseSwagger();
    app.UseSwaggerUI(options => options.DefaultModelsExpandDepth(-1));

    app.UseHttpsRedirection();
    app.Run();
}

public partial class Program;