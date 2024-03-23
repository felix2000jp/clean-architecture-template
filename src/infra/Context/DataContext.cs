using System.Reflection;
using core.Notes;
using Microsoft.EntityFrameworkCore;

namespace infra.Context;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public required DbSet<Note> Notes { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(executingAssembly);
    }
}