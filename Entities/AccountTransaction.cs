namespace Task1Bank.Entities;
using System.ComponentModel.DataAnnotations;

public class AccountTransaction
{
    [Key]
    public long TransactionId { get; set; }
    
    public int FromAccountId { get; set; }
    public int ToAccountId { get; set; }
    public string TransactionType { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } 
    public string Details { get; set; }
    public Account FromAccount { get; set; }
    public Account ToAccount { get; set; }
}

