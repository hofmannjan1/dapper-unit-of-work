using System.Data.Common;

namespace DapperUnitOfWork.MinimalExample.Data;

public interface IUnitOfWork : IAsyncDisposable
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

  public async ValueTask DisposeAsync()
  {
    await Context.Connection.DisposeAsync();
        if (Context.Transaction is not null) 
          await Context.Transaction.DisposeAsync();
    
        Context.IsDisposed = true;
  }
}
