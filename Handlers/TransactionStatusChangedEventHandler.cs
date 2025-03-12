using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task1Bank.Data;
using Task1Bank.Entities;
using Task1Bank.Events;

namespace Task1Bank.Handlers;


public class TransactionStatusChangedEventHandler : INotificationHandler<TransactionStatusChangedEvent>
{
    private readonly BankDBContext _context;
    private readonly ILogger<TransactionStatusChangedEventHandler> _logger;

    public TransactionStatusChangedEventHandler(BankDBContext context, ILogger<TransactionStatusChangedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(TransactionStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var version = await _context.TransactionEvents
                .Where(e => e.TransactionId == notification.TransactionId)
                .CountAsync(cancellationToken) + 1;

            var transactionEvent = new TransactionEvent
            {
                TransactionId = notification.TransactionId,
                EventType = "TransactionStatusChanged",
                Details = JsonSerializer.Serialize(new 
                {
                    notification.PreviousStatus,
                    notification.NewStatus
                }),
                Timestamp = notification.Timestamp,
                Version = version
            };

            _context.TransactionEvents.Add(transactionEvent);
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation($"Transaction status changed event stored for TransactionId: {notification.TransactionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling transaction status changed event for TransactionId: {notification.TransactionId}");
            throw;
        }
    }
}