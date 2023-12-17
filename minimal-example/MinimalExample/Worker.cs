using Dapper;
using DapperUnitOfWork.MinimalExample.Data;

namespace DapperUnitOfWork.MinimalExample;

public class Worker : IHostedService
{ 
  private readonly IServiceProvider _serviceProvider;

  public Worker(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;
  
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await using var scope = _serviceProvider.CreateAsyncScope();

    await SeedDatabase(scope);
  }

  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  private async Task SeedDatabase(AsyncServiceScope scope)
  {
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory>();
    
    await using var context = contextFactory.CreateContext();

    const string sql = @"
      CREATE TABLE IF NOT EXISTS Account (Id GUID NOT NULL PRIMARY KEY, Balance REAL, UNIQUE(Id));
      INSERT OR IGNORE INTO Account(Id, Balance) VALUES (@Id, @Balance);";

    var accounts = new List<object>
    {
      new { Id = Guid.NewGuid(), Balance = (decimal)500 },
      new { Id = Guid.NewGuid(), Balance = (decimal)800 }
    };

    await context.Connection.ExecuteAsync(sql, accounts);
  }
}
