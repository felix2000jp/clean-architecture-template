namespace core;

public interface IUnitOfWork
{
    public Task CommitChanges();
}