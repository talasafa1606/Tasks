namespace Task1Bank.Events;

using MediatR;
using System;

public abstract class TransactionDomainEvent : INotification
{
    public long TransactionId { get; set; }
    public DateTime Timestamp { get; set; }
    public int Version { get; set; }
    
    public TransactionDomainEvent()
    {
        Timestamp = DateTime.UtcNow;
    }
}
