using Task1Bank.Entities;

namespace Task1Bank.Data;
using Microsoft.EntityFrameworkCore;
public class BankDBContext: DbContext
{
    public BankDBContext(DbContextOptions<BankDBContext> options) : base(options) { }
    
    public DbSet<TransactionLog> TransactionLogs { get; set; }
    
}