namespace Task1Bank.Entities;
using System.ComponentModel.DataAnnotations;

public class AccountTransaction
{
    [Key]
    public long TransactionId { get; set; }
    
    public long AccountId { get; set; }
    public long UserId { get; set; }  
    public string TransactionType { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } 
    public string Details { get; set; }
}