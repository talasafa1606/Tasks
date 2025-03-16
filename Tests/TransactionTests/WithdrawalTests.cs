using Task1Bank.Entities;
using Task1Bank.Services;
using Task1Bank.Tests.TestHelpers;

namespace Task1Bank.Tests.TransactionTests;

using Xunit;
using Moq;
using System;
using System.Threading.Tasks;

public class WithdrawalTests
{
    [Fact]
    public async Task Withdraw_WithValidAmount_ShouldDecreaseBalance()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<BankingService>>();
        var bankingService = new BankingService(mockUnitOfWork.Object,mockLogger.Object);

        var account = TestDataGenerator.GenerateValidAccount();
        decimal initialBalance = account.Balance;
        decimal withdrawalAmount = 200;

        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);

        // Act
        await bankingService.WithdrawAsync(account.Id, withdrawalAmount, "Test withdrawal");

        // Assert
        Assert.Equal(initialBalance - withdrawalAmount, account.Balance);
        mockUnitOfWork.Verify(uow => uow.Accounts.GetByIdAsync(account.Id), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()),
            Times.Once);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task Withdraw_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<BankingService>>();
        var bankingService = new BankingService(mockUnitOfWork.Object,mockLogger.Object);

        var account = TestDataGenerator.GenerateValidAccount();
        decimal withdrawalAmount = -100;

        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await bankingService.WithdrawAsync(account.Id, withdrawalAmount, "Test withdrawal"));

        mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()),
            Times.Never);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task Withdraw_WithInsufficientBalance_ShouldThrowException()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<BankingService>>();
        var bankingService = new BankingService(mockUnitOfWork.Object,mockLogger.Object);
            
        var account = TestDataGenerator.GenerateValidAccount();
        decimal withdrawalAmount = account.Balance + 100; // more than the balance
            
        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
                
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await bankingService.WithdrawAsync(account.Id, withdrawalAmount, "Test withdrawal"));
                
        mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()), Times.Never);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
    }
        
    [Fact]
    public async Task Withdraw_WithNonExistentAccount_ShouldThrowException()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<BankingService>>();
        var bankingService = new BankingService(mockUnitOfWork.Object,mockLogger.Object);
    
        var accountId = Guid.NewGuid();
        decimal withdrawalAmount = 100;
            
        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId))
            .ReturnsAsync((Account)null);
                
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await bankingService.WithdrawAsync(accountId, withdrawalAmount, "Test withdrawal"));
                
        mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()), Times.Never);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
    }
}


