using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Task1Bank.Data;
using Task1Bank.Repositories;

namespace Task1Bank.UOF;

public class UnitOfWork : IUnitOfWork
{
    private readonly BankDBContext _context;
    private IDbContextTransaction _transaction;
    
    public IAccountRepository Accounts { get; private set; }
    public ITransactionRepository Transactions { get; private set; }
    
    public UnitOfWork(BankDBContext context)
    {
        _context = context;
        Accounts = new AccountRepository(_context);
        Transactions = new TransactionRepository(_context);
    }
    
    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        try
        {
            await _transaction.CommitAsync();
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
        finally
        {
            _transaction.Dispose();
        }
    }
    
    public async Task RollbackTransactionAsync()
    {
        await _transaction.RollbackAsync();
        _transaction.Dispose();
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}