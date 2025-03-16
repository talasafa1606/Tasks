using Task1Bank.Entities;
using Task1Bank.Entities.DTOs;

namespace Task1Bank.Repositories;


public interface IAccountRepository : IRepository<Account>
{
    Task<Account> GetByAccountNumberAsync(string accountNumber);
    Task<AccountDetailsDTO> GetAccountDetailsAsync(Guid accountId, string preferredLanguage);
    Task<bool> TransferFundsAsync(Guid fromAccountId, Guid toAccountId, decimal amount);
    
}
