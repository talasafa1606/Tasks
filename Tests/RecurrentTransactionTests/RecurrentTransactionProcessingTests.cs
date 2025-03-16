using Task1Bank.Tests.TestHelpers;

namespace Task1Bank.Tests.RecurrentTransactionTests;

using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RecurrentTransactionProcessingTests
{
    [Fact]
    public async Task ProcessRecurrentTransactions_ShouldProcessDueTransactions()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var transactionService = new TransactionService(mockUnitOfWork.Object);
        var bankingService = new BankingService(mockUnitOfWork.Object);
        
        var account = TestDataGenerator.GenerateValidAccount();
        var today = DateTime.UtcNow.Date;
        
        var recurrentTransactions = new List<RecurrentTransaction>
        {
            new RecurrentTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                Amount = 100,
                TransactionType = "Withdrawal",
                Frequency = "Monthly",
                NextExecutionDate = today, // Due today
                Description = "Monthly subscription"
            },
            new RecurrentTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                Amount = 200,
                TransactionType = "Deposit",
                Frequency = "Monthly",
                NextExecutionDate = today, // Due today
                Description = "Monthly salary"
            },
            new RecurrentTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                Amount = 50,
                TransactionType = "Withdrawal",
                Frequency = "Monthly",
                NextExecutionDate = today.AddDays(5), // Due in future
                Description = "Internet bill"
            }
        };
        
        mockUnitOfWork.Setup(uow => uow.TransactionRepository.GetDueRecurrentTransactionsAsync(today))
            .ReturnsAsync(recurrentTransactions.GetRange(0, 2)); // Only the first two are due
            
        mockUnitOfWork.Setup(uow => uow.AccountsRepository.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        // Act
        var result = await transactionService.ProcessDueRecurrentTransactionsAsync(today);
        
        // Assert
        Assert.Equal(2, result); // Should process 2 transactions
        mockUnitOfWork.Verify(uow => uow.TransactionRepository.GetDueRecurrentTransactionsAsync(today), Times.Once);
        mockUnitOfWork.Verify(uow => uow.TransactionRepository.UpdateRecurrentTransactionNextDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Exactly(2));
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task ProcessRecurrentTransactions_WithInsufficientFunds_ShouldSkipWithdrawal()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var transactionService = new TransactionService(mockUnitOfWork.Object);
        var bankingService = new BankingService(mockUnitOfWork.Object);
        
        var account = TestDataGenerator.GenerateValidAccount();
        account.Balance = 50; // Low balance
        var today = DateTime.UtcNow.Date;
        
        var recurrentTransactions = new List<RecurrentTransaction>
        {
            new RecurrentTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                Amount = 100, // More than account balance
                TransactionType = "Withdrawal",
                Frequency = "Monthly",
                NextExecutionDate = today,
                Description = "Monthly subscription"
            }
        };
        
        mockUnitOfWork.Setup(uow => uow.TransactionRepository.GetDueRecurrentTransactionsAsync(today))
            .ReturnsAsync(recurrentTransactions);
            
        mockUnitOfWork.Setup(uow => uow.AccountsRepository.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        // Mock error handling or failed transaction logging
        mockUnitOfWork.Setup(uow => uow.TransactionRepository.LogFailedRecurrentTransactionAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(true);
            
        // Act
        var result = await transactionService.ProcessDueRecurrentTransactionsAsync(today);
        
        // Assert
        Assert.Equal(0, result); // Should process 0 transactions successfully
        mockUnitOfWork.Verify(uow => uow.TransactionRepository.LogFailedRecurrentTransactionAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.AtLeastOnce);
    }
}
