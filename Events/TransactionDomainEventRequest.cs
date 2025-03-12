using System.Text.Json;

namespace Task1Bank.Events;

public class TransactionDomainEventRequest
{
    public long TransactionId { get; set; }
    public string EventType { get; set; }
    public JsonElement  Details { get; set; }
}