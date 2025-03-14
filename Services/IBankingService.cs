namespace Task1Bank.Services;

public interface IBankingService
{
    Task<(bool Success, string Message)> TransferFundsAsync(int fromAccountId, int toAccountId, decimal amount);
}
