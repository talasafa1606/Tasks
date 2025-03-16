using Task1Bank.Services;
using Task1Bank.Tests.TestHelpers;

namespace Task1Bank.Tests.TransactionTests;

using Xunit;
using Moq;
using System;
using System.Threading.Tasks;


public class BalanceUpdateTests
{
    [Fact]
    public async Task BalanceUpdate_AfterDeposit_ShouldBeCorrect()
    {
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLoggerB = new Mock<ILogger<BankingService>>();

        var bankingService = new BankingService(mockUnitOfWork.Object,mockLoggerB.Object);
        var mockLogger = new Mock<ILogger<AccountService>>();
        var mockLocalizer = new Mock<ILocalizationService>();
        var accountService = new AccountService(mockUnitOfWork.Object, mockLocalizer.Object, mockLogger.Object);

        var account = TestDataGenerator.GenerateValidAccount();
        decimal initialBalance = account.Balance;
        decimal depositAmount = 500;
        
        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        // Act
        await bankingService.DepositAsync(account.Id, depositAmount, "Test deposit");
        var updatedAccount = await accountService.GetAccountDetailsAsync(account.Id,"en");
        
        // Assert
        Assert.Equal<decimal>(initialBalance + depositAmount, updatedAccount.Balance);  // Explicitly specifying decimal
    }
    
    [Fact]
    public async Task BalanceUpdate_AfterWithdrawal_ShouldBeCorrect()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLoggerB = new Mock<ILogger<BankingService>>();

        var bankingService = new BankingService(mockUnitOfWork.Object,mockLoggerB.Object);
        var mockLogger = new Mock<ILogger<AccountService>>();
        var mockLocalizer = new Mock<ILocalizationService>();
        var accountService = new AccountService(mockUnitOfWork.Object, mockLocalizer.Object, mockLogger.Object);

        var account = TestDataGenerator.GenerateValidAccount();
        decimal initialBalance = account.Balance;
        decimal withdrawalAmount = 300;
        
        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        // Act
        await bankingService.WithdrawAsync(account.Id, withdrawalAmount, "Test withdrawal");
        var updatedAccount = await accountService.GetAccountDetailsAsync(account.Id,"en");
        
        // Assert
        Assert.Equal(initialBalance - withdrawalAmount, updatedAccount.Balance);
    }
    
    [Fact]
    public async Task BalanceUpdate_AfterMultipleTransactions_ShouldBeCorrect()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLoggerB = new Mock<ILogger<BankingService>>();

        var bankingService = new BankingService(mockUnitOfWork.Object,mockLoggerB.Object);
        var mockLogger = new Mock<ILogger<AccountService>>();
        var mockLocalizer = new Mock<ILocalizationService>();
        var accountService = new AccountService(mockUnitOfWork.Object, mockLocalizer.Object, mockLogger.Object);

        var account = TestDataGenerator.GenerateValidAccount();
        decimal initialBalance = account.Balance;
        decimal deposit1 = 500;
        decimal withdrawal = 300;
        decimal deposit2 = 200;
        
        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        // Act
        await bankingService.DepositAsync(account.Id, deposit1, "First deposit");
        await bankingService.WithdrawAsync(account.Id, withdrawal, "Withdrawal");
        await bankingService.DepositAsync(account.Id, deposit2, "Second deposit");
        
        var updatedAccount = await accountService.GetAccountDetailsAsync(account.Id,"en");
        
        // Assert
        decimal expectedBalance = initialBalance + deposit1 - withdrawal + deposit2;
        Assert.Equal(expectedBalance, updatedAccount.Balance);
    }
}
