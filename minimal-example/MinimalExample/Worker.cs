using Dapper;
using DapperUnitOfWork.MinimalExample.Data;

namespace DapperUnitOfWork.MinimalExample;

public class Worker : IHostedService
{ 
  private static readonly List<object> Accounts =
  [
    new { Id = "656F4B33-85CC-4546-AFD8-10468C88348C", Balance = (decimal)500 },
    new { Id = "062DFAF2-E83A-4F0E-8378-43076F17082A", Balance = (decimal)800 }
  ];

  private readonly IServiceProvider _serviceProvider;
  
  public Worker(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;
  
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await using var scope = _serviceProvider.CreateAsyncScope();

    // Uncomment the desired database provider.
    // await SeedSqliteDatabaseAsync(scope);
    await SeedSqlServerDatabaseAsync(scope);
  }

  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  private async Task SeedSqlServerDatabaseAsync(AsyncServiceScope scope)
  {
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory>();
        
    await using var context = contextFactory.CreateContext();

    const string sql = @"
      IF OBJECT_ID(N'dbo.Account', N'U') IS NULL
      CREATE TABLE Account (Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, Balance DECIMAL(19,4), UNIQUE(Id));
      CREATE TABLE #Account (Id UNIQUEIDENTIFIER, Balance DECIMAL(19,4));

      INSERT INTO #Account (Id, Balance) VALUES (@Id, @Balance);

      INSERT INTO Account(Id, Balance) 
      SELECT Id, Balance FROM #Account
      EXCEPT 
      SELECT Id, Balance FROM Account;";

    await context.Connection.ExecuteAsync(sql, Accounts);
  }

  private async Task SeedSqliteDatabaseAsync(AsyncServiceScope scope)
  {
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory>();
    
    await using var context = contextFactory.CreateContext();

    const string sql = @"
      CREATE TABLE IF NOT EXISTS Account (Id GUID NOT NULL PRIMARY KEY, Balance REAL, UNIQUE(Id));
      INSERT OR IGNORE INTO Account(Id, Balance) VALUES (@Id, @Balance);";

    await context.Connection.ExecuteAsync(sql, Accounts);
  }
}
