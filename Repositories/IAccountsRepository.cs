using Task1Bank.Entities;

namespace Task1Bank.Repositories;


public interface IAccountRepository : IRepository<Account>
{
    Task<Account> GetByAccountNumberAsync(string accountNumber);
    Task<bool> TransferFundsAsync(int fromAccountId, int toAccountId, decimal amount);
}
