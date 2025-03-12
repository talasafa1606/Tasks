using Task1Bank.Entities;

namespace Task1Bank.Data;
using Microsoft.EntityFrameworkCore;
public class BankDBContext: DbContext
{
    public BankDBContext(DbContextOptions<BankDBContext> options) : base(options) { }
    
    public DbSet<TransactionLog> TransactionLogs { get; set; }
    public DbSet<AccountTransaction> AccountTransactions { get; set; }
    public DbSet<Log> Logs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Log>()
            .HasIndex(l => l.RequestId);
            
        modelBuilder.Entity<Log>()
            .HasIndex(l => l.Timestamp);
            
        modelBuilder.Entity<Log>()
            .HasIndex(l => l.RouteURL);
    }
}