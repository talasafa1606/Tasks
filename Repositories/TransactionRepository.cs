using Microsoft.EntityFrameworkCore;
using Task1Bank.Data;
using Task1Bank.Entities;

namespace Task1Bank.Repositories;

public class TransactionRepository : Repository<AccountTransaction>, ITransactionRepository
{
    public TransactionRepository(BankDBContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<AccountTransaction>> GetByAccountIdAsync(int accountId)
    {
        return await _context.Set<AccountTransaction>()
            .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId)
            .ToListAsync();
    }
}