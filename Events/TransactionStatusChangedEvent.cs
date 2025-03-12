namespace Task1Bank.Events;

public class TransactionStatusChangedEvent : TransactionDomainEvent
{
    public string PreviousStatus { get; set; }
    public string NewStatus { get; set; }
}

