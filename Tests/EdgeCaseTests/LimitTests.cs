using Task1Bank.Entities;
using Task1Bank.Services;
using Task1Bank.Tests.TestHelpers;

namespace Task1Bank.Tests.EdgeCaseTests;

using Xunit;
using Moq;
using System;
using System.Threading.Tasks;


public class LimitTests
{
    [Fact]
    public async Task Deposit_WithMaximumAmount_ShouldSucceed()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var bankingService = new BankingService(mockUnitOfWork.Object);
        
        var account = TestDataGenerator.GenerateValidAccount();
        decimal initialBalance = account.Balance;
        decimal maxDepositAmount = 1000000; // Assuming this is within system limits
        
        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        // Act
        await bankingService.DepositAsync(account.Id, maxDepositAmount, "Maximum deposit test");
        
        // Assert
        Assert.Equal(initialBalance + maxDepositAmount, account.Balance);
        mockUnitOfWork.Verify(uow => uow.Accounts.GetByIdAsync(account.Id), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }
    
    [Fact]
    public async Task Deposit_ExceedingMaximumLimit_ShouldThrowException()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var bankingService = new BankingService(mockUnitOfWork.Object);
        
        var account = TestDataGenerator.GenerateValidAccount();
        decimal excessiveDepositAmount = decimal.MaxValue; // System limit exceeded
        
        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => 
            await bankingService.DepositAsync(account.Id, excessiveDepositAmount, "Excessive deposit test"));
            
        mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()), Times.Never);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
    }
    
    [Fact]
    public async Task Withdraw_AtMinimumBalance_ShouldSucceed()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var bankingService = new BankingService(mockUnitOfWork.Object);
        
        // Account with minimum required balance
        var account = new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = "ACC" + DateTime.Now.Ticks.ToString().Substring(0, 10),
            Balance = 100, // Minimum balance required
            CreatedDate = DateTime.UtcNow,
            LastUpdatedDate = DateTime.UtcNow,
            IsActive = true
        };
        
        decimal withdrawalAmount = 50; // Half of the balance
        
        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        // Act
        await bankingService.WithdrawAsync(account.Id, withdrawalAmount, "Minimum balance withdrawal test");
        
        // Assert
        Assert.Equal(50, account.Balance);
        mockUnitOfWork.Verify(uow => uow.Accounts.GetByIdAsync(account.Id), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }
    
    [Fact]
    public async Task Withdraw_BelowMinimumBalance_ShouldThrowException()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var bankingService = new BankingService(mockUnitOfWork.Object);
        
        // Account with minimum required balance
        var account = new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = "ACC" + DateTime.Now.Ticks.ToString().Substring(0, 10),
            Balance = 100, 
            CreatedDate = DateTime.UtcNow,
            LastUpdatedDate = DateTime.UtcNow,
            IsActive = true,
        };
        
        decimal withdrawalAmount = 60; 
        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await bankingService.WithdrawAsync(account.Id, withdrawalAmount, "Below minimum balance test"));
            
        mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()), Times.Never);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
    }
    
    [Fact]
    public async Task EventRollback_WithMaximumTimeRange_ShouldSucceed()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var eventService = new EventService(mockUnitOfWork.Object);
        
        var accountId = Guid.NewGuid();
        DateTime startDate = DateTime.UtcNow.AddDays(-30); // Maximum allowed rollback period
        DateTime endDate = DateTime.UtcNow;
        
        mockUnitOfWork.Setup(uow => uow.EventRepository.RollbackEventsAsync(accountId, startDate, endDate))
            .ReturnsAsync(true);
            
        // Act
        var result = await eventService.RollbackEventsAsync(accountId, startDate, endDate);
        
        // Assert
        Assert.True(result);
        mockUnitOfWork.Verify(uow => uow.EventRepository.RollbackEventsAsync(accountId, startDate, endDate), Times.Once);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task EventRollback_ExceedingMaximumTimeRange_ShouldThrowException()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var eventService = new EventService(mockUnitOfWork.Object);
        
        var accountId = Guid.NewGuid();
        DateTime startDate = DateTime.UtcNow.AddDays(-60); // Exceeds maximum allowed rollback period
        DateTime endDate = DateTime.UtcNow;
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => 
            await eventService.RollbackEventsAsync(accountId, startDate, endDate));
            
        mockUnitOfWork.Verify(uow => uow.EventRepository.RollbackEventsAsync(accountId, startDate, endDate), Times.Never);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
