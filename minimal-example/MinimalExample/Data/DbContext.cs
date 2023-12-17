using System.Data.Common;

namespace DapperUnitOfWork.MinimalExample.Data;

public interface IDbContext : IAsyncDisposable
{
  public DbConnection Connection { get; }
  public DbTransaction? Transaction { get; set; }
  bool IsDisposed { get; set; }
}

public class DbContext : IDbContext
{ 
  public DbConnection Connection { get; }
  public DbTransaction? Transaction { get; set; }
  public bool IsDisposed { get; set; }

  private readonly bool _isUnitOfWorkContext;

  public DbContext(DbConnection connection, bool isUnitOfWorkContext = false)
  {
    Connection = connection;
    Connection.Open();

    _isUnitOfWorkContext = isUnitOfWorkContext;
  }

  public async ValueTask DisposeAsync()
  {
    if (_isUnitOfWorkContext)
      return;
    
    await Connection.DisposeAsync();
    if (Transaction is not null) 
      await Transaction.DisposeAsync();

    IsDisposed = true;
  }
}
