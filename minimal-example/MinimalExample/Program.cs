using DapperUnitOfWork.MinimalExample;
using DapperUnitOfWork.MinimalExample.Data;
using DapperUnitOfWork.MinimalExample.Repositories;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddQuartz();
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
builder.Services.AddHostedService<Worker>();

// Uncomment the desired database provider.
// builder.Services.AddScoped<IDbContextFactory, SqliteContextFactory>();
builder.Services.AddScoped<IDbContextFactory, SqlServerContextFactory>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{ 
  app.UseSwagger();
  app.UseSwaggerUI();
}

// Simple query example (single operation, no transaction)
app.MapGet("/accounts/{accountId:guid}/balance", (Guid accountId, CancellationToken cancellationToken, 
  IAccountRepository accountRepository) => accountRepository.GetBalanceAsync(accountId, cancellationToken));

// Complex command example (multiple operations, single transaction)
app.MapPost("/accounts/transfer", async (TransferBalanceRequest request, CancellationToken cancellationToken, 
  IDbContextFactory contextFactory, IAccountRepository accountRepository) =>
  {
    await using var unitOfWork = contextFactory.CreateUnitOfWork();

    try
    {
      await unitOfWork.BeginAsync();

      await accountRepository.WithdrawAmountAsync(request.SourceAccountId, request.Amount, cancellationToken);
      await accountRepository.DepositAmountAsync(request.TargetAccountId, request.Amount, cancellationToken);

      await unitOfWork.CommitAsync();
    }
    catch (Exception)
    {
      await unitOfWork.RollbackAsync();
      throw;
    }
  });

app.UseHttpsRedirection();
app.Run();

public record TransferBalanceRequest(Guid SourceAccountId, Guid TargetAccountId, decimal Amount);
