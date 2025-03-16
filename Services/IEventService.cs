using Task1Bank.Entities.DTOs;

namespace Task1Bank.Services;

using Task1Bank.Entities;
using Task1Bank.Events;
using System.Threading.Tasks;

public interface IEventService
{
    Task<TransactionEvent> CreateAndDispatchEvent(TransactionDomainEventRequest request);
    Task<List<TransactionNotificationDTO>> GetUserNotificationsAsync(Guid userId);
    Task<IEnumerable<TransactionEvent>> GetEventsByTransactionId(Guid transactionId);
    
}