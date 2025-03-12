using System.ComponentModel.DataAnnotations;

namespace Task1Bank.Entities;

public class TransactionLogRequest
{
    [Required]
    public long AccountId { get; set; }
    
    [Required]
    public string TransactionType { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public string Status { get; set; }
    
    public string Details { get; set; }
}