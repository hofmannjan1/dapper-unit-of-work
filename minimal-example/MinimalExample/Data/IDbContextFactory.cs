namespace DapperUnitOfWork.MinimalExample.Data;

public interface IDbContextFactory
{ 
  IDbContext CreateContext();
  IUnitOfWork CreateUnitOfWork();
}
