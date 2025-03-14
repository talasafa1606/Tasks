namespace Task1Bank.Entities.DTOs;

public class TransactionNotificationDTO
{
    public string Title { get; set; }
    public string TransactionTypeLabel { get; set; }
    public string TransactionType { get; set; }
    public string TransactionDateLabel { get; set; }
    public string TransactionDate { get; set; }
    public string AmountLabel { get; set; }
    public string Amount { get; set; }
    public string Description { get; set; }
}