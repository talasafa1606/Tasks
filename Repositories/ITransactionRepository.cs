using Task1Bank.Entities;

namespace Task1Bank.Repositories;

public interface ITransactionRepository : IRepository<AccountTransaction>
{
    Task<IEnumerable<AccountTransaction>> GetByAccountIdAsync(int accountId);
}