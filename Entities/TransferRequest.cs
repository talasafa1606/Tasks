using System.ComponentModel.DataAnnotations;

namespace Task1Bank.Entities;

public class TransferRequest
{
    [Required]
    public Guid FromAccountId { get; set; }
    
    [Required]
    public Guid ToAccountId { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }
}