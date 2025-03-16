namespace Task1Bank.Entities.DTOs;

public class TransactionNotificationDTO
{
    public Guid TransactionId { get; set; }
    public string Title { get; set; }
    public string TransactionTypeLabel { get; set; }
    public string TransactionType { get; set; }
    public DateTime TransactionDateLabel { get; set; }
    public string TransactionDate { get; set; }
    public decimal AmountLabel { get; set; }
    public string Amount { get; set; }
    public string Description { get; set; }
    public bool IsRead { get; set; }
}