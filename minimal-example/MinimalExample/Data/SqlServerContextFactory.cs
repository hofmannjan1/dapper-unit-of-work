using Microsoft.Data.SqlClient;

namespace DapperUnitOfWork.MinimalExample.Data;

public class SqlServerContextFactory : IDbContextFactory
{
  private readonly string? _connectionString;
  private IUnitOfWork? _unitOfWork;
  private bool IsUnitOfWorkActive => _unitOfWork is not null && !_unitOfWork.IsDisposed;

  public SqlServerContextFactory(IConfiguration configuration) => 
    _connectionString = configuration.GetConnectionString("SqlServer");

  public IDbContext CreateContext() =>
    IsUnitOfWorkActive ? _unitOfWork!.Context : new DbContext(new SqlConnection(_connectionString));

  public IUnitOfWork CreateUnitOfWork()
  {
    if (IsUnitOfWorkActive)
      throw new InvalidOperationException(
        "Could not begin a unit of work because there already exist an active unit of work.");

    return _unitOfWork = new UnitOfWork(new SqlConnection(_connectionString));
  }
}
