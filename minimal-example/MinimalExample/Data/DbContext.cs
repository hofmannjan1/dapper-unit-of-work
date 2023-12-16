using System.Data.Common;

namespace DapperUnitOfWork.MinimalExample.Data;

public interface IDbContext : IDisposable
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

  public void Dispose()
  {
    if (_isUnitOfWorkContext)
      return;

    Transaction?.Dispose();
    Connection?.Dispose();
    IsDisposed = true;
  }
}
