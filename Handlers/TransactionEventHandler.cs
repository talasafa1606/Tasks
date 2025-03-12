using Microsoft.EntityFrameworkCore;
using Task1Bank.Data;
using Task1Bank.Entities;
using Task1Bank.Events;

namespace Task1Bank.Handlers;

using MediatR;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class TransactionCreatedEventHandler : INotificationHandler<TransactionCreatedEvent>
{
    private readonly BankDBContext _context;
    private readonly ILogger<TransactionCreatedEventHandler> _logger;

    public TransactionCreatedEventHandler(BankDBContext context, ILogger<TransactionCreatedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var version = await _context.TransactionEvents
                .Where(e => e.TransactionId == notification.TransactionId)
                .CountAsync(cancellationToken) + 1;

            var transactionEvent = new TransactionEvent
            {
                TransactionId = notification.TransactionId,
                EventType = "TransactionCreated",
                Details = JsonSerializer.Serialize(new 
                {
                    notification.AccountId,
                    notification.TransactionType,
                    notification.Amount
                }),
                Timestamp = notification.Timestamp,
                Version = version
            };

            _context.TransactionEvents.Add(transactionEvent);
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation($"Transaction created event stored for TransactionId: {notification.TransactionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling transaction created event for TransactionId: {notification.TransactionId}");
            throw;
        }
    }
}
