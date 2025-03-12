namespace Task1Bank.Events;


public class TransactionCreatedEvent : TransactionDomainEvent
{
    public long AccountId { get; set; }
    public string TransactionType { get; set; }
    public decimal Amount { get; set; }
}

