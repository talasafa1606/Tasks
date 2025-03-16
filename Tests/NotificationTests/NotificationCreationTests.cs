using Task1Bank.Entities.DTOs;
using Task1Bank.Services;
using Task1Bank.Tests.TestHelpers;

namespace Task1Bank.Tests.NotificationTests;

using Xunit;
using Moq;
using System;
using System.Threading.Tasks;


public class NotificationCreationTests
{
    [Fact]
    public async Task CreateNotification_ForDeposit_ShouldCreateNotification()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<TransactionService>>();
        var mockLocalizer = new Mock<ILocalizationService>();

        var transactionService = new TransactionService(mockUnitOfWork.Object, mockLocalizer.Object, mockLogger.Object);

        var account = TestDataGenerator.GenerateValidAccount();
        var transaction = TestDataGenerator.GenerateDeposit(account.Id);

        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
    
        // Mock the CreateNotificationAsync method to return a successful transaction notification
        mockUnitOfWork.Setup(uow => uow.Transactions.CreateNotificationAsync(It.IsAny<TransactionNotificationDTO>()))
            .ReturnsAsync(new TransactionNotificationDTO
            {
                // Here, you can populate the properties of the DTO based on your needs
                Title = "Test Notification",
                TransactionType = "Deposit",
                Amount = "$100",
                Description = "Deposit notification"
            });

        // Act
        var result = await transactionService.CreateTransactionNotificationAsync(transaction);

        // Assert
        Assert.True(result); // Expecting true since the notification is created successfully
        mockUnitOfWork.Verify(uow => uow.Transactions.CreateNotificationAsync(It.IsAny<TransactionNotificationDTO>()), Times.Once);
    }

    [Fact]
    public async Task CreateNotification_ForWithdrawal_ShouldCreateNotification()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<TransactionService>>();
        var mockLocalizer = new Mock<ILocalizationService>();

        var transactionService = new TransactionService(mockUnitOfWork.Object, mockLocalizer.Object, mockLogger.Object);

        var account = TestDataGenerator.GenerateValidAccount();
        var transaction = TestDataGenerator.GenerateWithdrawal(account.Id);

        mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
            .ReturnsAsync(account);

        // Mock the CreateNotificationAsync method to return a successful transaction notification
        mockUnitOfWork.Setup(uow => uow.Transactions.CreateNotificationAsync(It.IsAny<TransactionNotificationDTO>()))
            .ReturnsAsync(new TransactionNotificationDTO
            {
                // Populate with expected data for withdrawal
                Title = "Test Notification",
                TransactionType = "Withdrawal",
                Amount = "$50",
                Description = "Withdrawal notification"
            });

        // Act
        var result = await transactionService.CreateTransactionNotificationAsync(transaction);

        // Assert
        Assert.True(result); // Expecting true since the notification is created successfully
        mockUnitOfWork.Verify(uow => uow.Transactions.CreateNotificationAsync(It.IsAny<TransactionNotificationDTO>()), Times.Once);
    }
}
