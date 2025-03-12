using System.ComponentModel.DataAnnotations;

namespace Task1Bank.Entities;

public class TransactionLog
{
    [Key]
    public long Id { get; set; }
    public long AccountId { get; set; }
    public string TransactionType { get; set; } 
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } 
    public string Details { get; set; }
}