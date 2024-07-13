using core;
using infra.Context;

namespace infra;

public class UnitOfWork(DataContext dataContext) : IUnitOfWork
{
    private readonly DataContext _dataContext = dataContext;

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }
}