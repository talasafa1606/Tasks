using Task1Bank.Entities.DTOs;

namespace Task1Bank.Services;

public interface ITransactionService
{
    Task<TransactionNotificationDTO> GetTransactionNotificationAsync(int transactionId, string language);
}
