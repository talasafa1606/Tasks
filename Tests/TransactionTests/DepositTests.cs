using Task1Bank.Entities;
using Task1Bank.Services;
using Task1Bank.Tests.TestHelpers;

namespace Task1Bank.Tests.TransactionTests;

using Xunit;
using Moq;
using System;
using System.Threading.Tasks;



public class DepositTests
{
    [Fact]
    public async Task Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<BankingService>>();
        var bankingService = new BankingService(mockUnitOfWork.Object,mockLogger.Object);

        var account = TestDataGenerator.GenerateValidAccount();
        decimal initialBalance = account.Balance;
        decimal depositAmount = 200;

        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);

        // Act
        await bankingService.DepositAsync(account.Id, depositAmount, "Test deposit");

        // Assert
        Assert.Equal(initialBalance + depositAmount, account.Balance);
        mockUnitOfWork.Verify(uow => uow.Accounts.GetByIdAsync(account.Id), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()),
            Times.Once);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task Deposit_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<BankingService>>();
        var bankingService = new BankingService(mockUnitOfWork.Object,mockLogger.Object);

        var account = TestDataGenerator.GenerateValidAccount();
        decimal depositAmount = -100;

        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await bankingService.DepositAsync(account.Id, depositAmount, "Test deposit"));

        mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()),
            Times.Never);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task Deposit_WithNonExistentAccount_ShouldThrowException()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<BankingService>>();
        var bankingService = new BankingService(mockUnitOfWork.Object,mockLogger.Object);

        var accountId = Guid.NewGuid();
        decimal depositAmount = 100;

        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(accountId))
            .ReturnsAsync((Account)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await bankingService.DepositAsync(accountId, depositAmount, "Test deposit"));

        mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()),
            Times.Never);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
    }
}