using Microsoft.EntityFrameworkCore;
using Task1Bank.Data;
using Task1Bank.Entities;

namespace Task1Bank.Repositories;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(BankDBContext context) : base(context)
    {
    }

    public async Task<Account> GetByAccountNumberAsync(string accountNumber)
    {
        return await _context.Set<Account>()
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<bool> TransferFundsAsync(int fromAccountId, int toAccountId, decimal amount)
    {
        throw new NotImplementedException("This should be handled by a service using UnitOfWork");
    }
}