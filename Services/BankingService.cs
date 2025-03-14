﻿using Task1Bank.Entities;
using Task1Bank.UOF;

namespace Task1Bank.Services;

public class BankingService : IBankingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BankingService> _logger;
    
    public BankingService(IUnitOfWork unitOfWork, ILogger<BankingService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<(bool Success, string Message)> TransferFundsAsync(int fromAccountId, int toAccountId, decimal amount)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            
            var fromAccount = await _unitOfWork.Accounts.GetByIdAsync(fromAccountId);
            var toAccount = await _unitOfWork.Accounts.GetByIdAsync(toAccountId);
            
            if (fromAccount == null)
                return (false, "Source account not found");
                
            if (toAccount == null)
                return (false, "Destination account not found");
                
            if (!fromAccount.IsActive || !toAccount.IsActive)
                return (false, "One or both accounts are inactive");
                
            if (fromAccount.Balance < amount)
                return (false, "Insufficient funds");
                
            fromAccount.Balance -= amount;
            toAccount.Balance += amount;
            fromAccount.LastUpdatedDate = DateTime.UtcNow;
            toAccount.LastUpdatedDate = DateTime.UtcNow;
            
            var transaction = new AccountTransaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = amount,
                Timestamp = DateTime.UtcNow,
                TransactionType = "Transfer",
                Details = $"Transfer from {fromAccount.AccountNumber} to {toAccount.AccountNumber}",
                Status = "Completed"
            };
            
            await _unitOfWork.Transactions.AddAsync(transaction);
            
            await _unitOfWork.CompleteAsync();
            
            await _unitOfWork.CommitTransactionAsync();
            
            return (true, $"Successfully transferred ${amount} from account {fromAccount.AccountNumber} to {toAccount.AccountNumber}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during fund transfer");
            
            await _unitOfWork.RollbackTransactionAsync();
            
            return (false, "An error occurred during the transfer");
        }
    }
}