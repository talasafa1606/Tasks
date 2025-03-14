using Task1Bank.Entities;

namespace Task1Bank.Data;
using Microsoft.EntityFrameworkCore;
public class BankDBContext: DbContext
{
    public BankDBContext(DbContextOptions<BankDBContext> options) : base(options) { }
    
    public DbSet<TransactionLog> TransactionLogs { get; set; }
    public DbSet<AccountTransaction> AccountTransactions { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<TransactionEvent> TransactionEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Log>()
            .HasIndex(l => l.RequestId);
            
        modelBuilder.Entity<Log>()
            .HasIndex(l => l.Timestamp);
            
        modelBuilder.Entity<Log>()
            .HasIndex(l => l.RouteURL);
        
        
        modelBuilder.Entity<Account>()
            .HasKey(a => a.Id);
            
        modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasColumnType("decimal(18,2)");
            
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.AccountNumber)
            .IsUnique();
            
        modelBuilder.Entity<AccountTransaction>()
            .HasKey(t => t.TransactionId);
            
        modelBuilder.Entity<AccountTransaction>()
            .Property(t => t.Amount)
            .HasColumnType("decimal(18,2)");
            
        modelBuilder.Entity<AccountTransaction>()
            .HasOne(t => t.FromAccount)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.FromAccountId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<AccountTransaction>()
            .HasOne(t => t.ToAccount)
            .WithMany()
            .HasForeignKey(t => t.ToAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}