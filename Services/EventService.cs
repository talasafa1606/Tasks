using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task1Bank.Data;
using Task1Bank.Entities;
using Task1Bank.Entities.DTOs;
using Task1Bank.Events;

namespace Task1Bank.Services;

public class EventService : IEventService
{
    private readonly BankDBContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<EventService> _logger;
    private IEventService _eventServiceImplementation;

    public EventService(BankDBContext context, IMediator mediator, ILogger<EventService> logger)
    {
        _context = context;
        _mediator = mediator;
        _logger = logger;
    }
    /*
    public async Task<TransactionEvent> CreateAndDispatchEvent(TransactionDomainEventRequest request)
    {
        TransactionDomainEvent domainEvent = CreateDomainEvent(request);
        
        if (domainEvent == null)
        {
            throw new ArgumentException($"Unsupported event type: {request.EventType}");
        }
        
        await _mediator.Publish(domainEvent);
        
        var latestEvent = await _context.TransactionEvents
            .Where(e => e.TransactionId == request.TransactionId)
            .OrderByDescending(e => e.Version)
            .FirstOrDefaultAsync();
            
        return latestEvent;
    }
    */

    public Task<TransactionEvent> CreateAndDispatchEvent(TransactionDomainEventRequest request)
    {
        return _eventServiceImplementation.CreateAndDispatchEvent(request);
    }

    public Task<List<TransactionNotificationDTO>> GetUserNotificationsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TransactionEvent>> GetEventsByTransactionId(Guid transactionId)
    {
        return await _context.TransactionEvents
            .Where(e => e.TransactionId == transactionId)
            .OrderBy(e => e.Version)
            .ToListAsync();
    }
    public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId)
    {
        var notification = await _context.Set<TransactionEvent>()
            .FirstOrDefaultAsync(n => n.TransactionId == notificationId);
        
        if (notification == null)
        {
            return false;
        }
        
        notification.isRead = true;
        _context.Update(notification);
        await _context.SaveChangesAsync();
        
        return true;
    }
    private TransactionDomainEvent CreateDomainEvent(TransactionDomainEventRequest request)
    {
        try
        {
            var eventDetails = JsonSerializer.Deserialize<JsonElement>(request.Details);

            switch (request.EventType.ToLowerInvariant())
            {
                case "transactioncreated":
                case "deposit":  // ✅ Treat "Deposit" as a TransactionCreatedEvent
                    return new TransactionCreatedEvent
                    {
                        TransactionId = request.TransactionId,
                        AccountId = eventDetails.GetProperty("accountId").GetInt64(),
                        TransactionType = request.EventType,  // Store event type dynamically
                        Amount = eventDetails.GetProperty("amount").GetDecimal()
                    };

                case "transactionstatuschanged":
                    return new TransactionStatusChangedEvent
                    {
                        TransactionId = request.TransactionId,
                        PreviousStatus = eventDetails.GetProperty("previousStatus").GetString(),
                        NewStatus = eventDetails.GetProperty("newStatus").GetString()
                    };

                default:
                    _logger.LogWarning($"Unsupported event type: {request.EventType}");
                    throw new ArgumentException($"Unsupported event type: {request.EventType}");
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON format in Details field.");
            throw new ArgumentException("Invalid JSON format in Details field.");
        }
    }

}