using Dapper;
using DapperUnitOfWork.MinimalExample.Data;

namespace DapperUnitOfWork.MinimalExample.Repositories;

public interface IAccountRepository
{ 
  Task<decimal> GetBalanceAsync(Guid accountId, CancellationToken cancellationToken);
  Task DepositAmountAsync(Guid accountId, decimal amount, CancellationToken cancellationToken);
  Task WithdrawAmountAsync(Guid accountId, decimal amount, CancellationToken cancellationToken);
}

public class AccountRepository : IAccountRepository
{
  private readonly IDbContextFactory _contextFactory;
  
  public AccountRepository(IDbContextFactory contextFactory) => _contextFactory = contextFactory;
  
  public async Task<decimal> GetBalanceAsync(Guid accountId, CancellationToken cancellationToken) 
  {
    await using var context = _contextFactory.CreateContext();
    
    const string sql = "SELECT Balance FROM Account WHERE Id = @accountId";

    return await context.Connection.QuerySingleAsync<decimal>(new CommandDefinition(sql, new { accountId }, 
      transaction: context.Transaction, cancellationToken: cancellationToken));
  }
    
  public async Task DepositAmountAsync(Guid accountId, decimal amount, CancellationToken cancellationToken) 
  {
    await using var context = _contextFactory.CreateContext();

    const string sql = "UPDATE Account SET Balance = Balance + @amount WHERE Id = @accountId";

    await context.Connection.ExecuteAsync(new CommandDefinition(sql, new { accountId, amount },
      transaction: context.Transaction, cancellationToken: cancellationToken));
  }

  public async Task WithdrawAmountAsync(Guid accountId, decimal amount, CancellationToken cancellationToken)
  {
    await using var context = _contextFactory.CreateContext();

    const string sql = "UPDATE Account SET Balance = Balance - @amount WHERE Id = @accountId";

    await context.Connection.ExecuteAsync(new CommandDefinition(sql, new { accountId, amount },
      transaction: context.Transaction, cancellationToken: cancellationToken));
  }
}
