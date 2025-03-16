namespace Task1Bank.Services;

public interface IBankingService
{
    Task<(bool Success, string Message)> TransferFundsAsync(Guid fromAccountId, Guid toAccountId, decimal amount);
}
