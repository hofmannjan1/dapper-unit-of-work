using Microsoft.Data.Sqlite;

namespace DapperUnitOfWork.MinimalExample.Data;

public class SqliteContextFactory : IDbContextFactory
{
  private readonly string? _connectionString;
  private IUnitOfWork? _unitOfWork;
  private bool IsUnitOfWorkActive => _unitOfWork is not null && !_unitOfWork.IsDisposed;

  public SqliteContextFactory(IConfiguration configuration) => 
    _connectionString = configuration.GetConnectionString("Sqlite");

  public IDbContext CreateContext() =>
    IsUnitOfWorkActive ? _unitOfWork!.Context : new DbContext(new SqliteConnection(_connectionString));

  public IUnitOfWork CreateUnitOfWork()
  {
    if (IsUnitOfWorkActive)
      throw new InvalidOperationException(
        "Could not begin a unit of work because there already exist an active unit of work.");

    return _unitOfWork = new UnitOfWork(new SqliteConnection(_connectionString));
  }
}
