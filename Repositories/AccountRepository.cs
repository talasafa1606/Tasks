using Microsoft.EntityFrameworkCore;
using Task1Bank.Data;
using Task1Bank.Entities;
using Task1Bank.Entities.DTOs;

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

        public async Task<AccountDetailsDTO> GetAccountDetailsAsync(Guid accountId, string preferredLanguage)
        {
            var account = await _context.Set<Account>()
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
                return null;

            // Mapping Account to AccountDetailsDTO
            var accountDetails = new AccountDetailsDTO
            {
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
            };

            return accountDetails;
        }

        public async Task<bool> TransferFundsAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccount = await _context.Set<Account>()
                .FirstOrDefaultAsync(a => a.Id == fromAccountId);

            var toAccount = await _context.Set<Account>()
                .FirstOrDefaultAsync(a => a.Id == toAccountId);

            if (fromAccount == null || toAccount == null)
                return false; // One or both accounts do not exist

            if (fromAccount.Balance < amount)
                return false; // Insufficient funds

            // Perform the transfer
            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            _context.Set<Account>().Update(fromAccount);
            _context.Set<Account>().Update(toAccount);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true; // Successful transfer
        }
    }