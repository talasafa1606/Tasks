using Task1Bank.Entities;
using Task1Bank.Entities.DTOs;

namespace Task1Bank.Repositories;

public interface ITransactionRepository : IRepository<AccountTransaction>
{
    Task<IEnumerable<AccountTransaction>> GetByAccountIdAsync(Guid accountId);

    Task<TransactionNotificationDTO> CreateNotificationAsync(TransactionNotificationDTO transactionId);

    Task<List<TransactionNotificationDTO>> GetNotificationsByUserIdAsync(Guid userId);
    Task<bool> MarkNotificationAsReadAsync(Guid notificationId);

}