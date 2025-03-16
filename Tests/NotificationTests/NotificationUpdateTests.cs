using Task1Bank.Services;
using Task1Bank.Tests.TestHelpers;

namespace Task1Bank.Tests.NotificationTests;

using Xunit;
using Moq;
using System;
using System.Threading.Tasks;

public class NotificationUpdateTests
{
    [Fact]
    public async Task MarkNotificationAsRead_ShouldUpdateNotification()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<TransactionService>>();
        var mockLocalizer = new Mock<ILocalizationService>();

        var transactionService = new TransactionService(mockUnitOfWork.Object, mockLocalizer.Object, mockLogger.Object);
        
        var notificationId = Guid.NewGuid();
        
        mockUnitOfWork.Setup(uow => uow.Transactions.MarkNotificationAsReadAsync(notificationId))
            .ReturnsAsync(true);
            
        // Act
        var result = await transactionService.MarkNotificationAsReadAsync(notificationId);
        
        // Assert
        Assert.True(result);
        mockUnitOfWork.Verify(uow => uow.Transactions.MarkNotificationAsReadAsync(notificationId), Times.Once);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }
    
    [Fact]
    public async Task MarkNotificationAsRead_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var transactionService = new TransactionService(mockUnitOfWork.Object);
        
        var invalidNotificationId = Guid.NewGuid();
        
        mockUnitOfWork.Setup(uow => uow.Transactions.MarkNotificationAsReadAsync(invalidNotificationId))
            .ReturnsAsync(false);
            
        // Act
        var result = await transactionService.MarkNotificationAsReadAsync(invalidNotificationId);
        
        // Assert
        Assert.False(result);
        mockUnitOfWork.Verify(uow => uow.Transactions.MarkNotificationAsReadAsync(invalidNotificationId), Times.Once);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
    }
}
