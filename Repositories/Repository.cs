using Microsoft.EntityFrameworkCore;
using Task1Bank.Data;

namespace Task1Bank.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly BankDBContext _context;
    
    public Repository(BankDBContext context)
    {
        _context = context;
    }
    
    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }
    
    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }
    
    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
    
    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}
