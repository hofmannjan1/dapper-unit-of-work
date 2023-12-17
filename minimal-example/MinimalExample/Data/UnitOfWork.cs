using System.Data.Common;

namespace DapperUnitOfWork.MinimalExample.Data;

public interface IUnitOfWork : IDisposable
{ 
  DbContext Context { get; }
  bool IsDisposed { get; }
  Task BeginAsync();
  Task RollbackAsync();
  Task CommitAsync();
}

public class UnitOfWork : IUnitOfWork
{
  public DbContext Context { get; }
  public bool IsDisposed => Context.IsDisposed;

  public UnitOfWork(DbConnection connection) => Context = new DbContext(connection, true);

  public async Task BeginAsync() => 
    Context.Transaction = await Context.Connection.BeginTransactionAsync();

  public Task RollbackAsync() => Context.Transaction!.RollbackAsync();

  public Task CommitAsync() => Context.Transaction!.CommitAsync();

  public void Dispose()
  {
    Context.Connection?.Dispose();
    Context.Transaction?.Dispose();
    Context.IsDisposed = true;
  }
}
