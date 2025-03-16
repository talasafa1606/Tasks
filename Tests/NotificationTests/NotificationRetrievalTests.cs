using Task1Bank.Entities.DTOs;
using Task1Bank.Services;
using Task1Bank.Tests.TestHelpers;

namespace Task1Bank.Tests.NotificationTests;

using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NotificationRetrievalTests
{
    [Fact]
    public async Task GetNotifications_ForUser_ShouldReturnNotifications()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<TransactionService>>();
        var mockLocalizer = new Mock<ILocalizationService>();

        var transactionService = new TransactionService(mockUnitOfWork.Object, mockLocalizer.Object, mockLogger.Object);
        
        Guid userId = Guid.NewGuid();
        var notifications = new List<TransactionNotificationDTO>
        {
            new TransactionNotificationDTO 
            { 
                Title =userId.ToString(),
                Description = "Deposit of $100 completed",
                TransactionDateLabel = DateTime.UtcNow,
                IsRead = false
            },
            new TransactionNotificationDTO 
            { 
                Title =userId.ToString(),
                Description = "Withdrawal of $100 completed",
                TransactionDateLabel = DateTime.UtcNow,
                IsRead = false
            }
        };
        
        mockUnitOfWork.Setup(uow => uow.Transactions.GetNotificationsByUserIdAsync(userId))
            .ReturnsAsync(notifications);
            
        // Act
        var result = await transactionService.GetUserNotificationsAsync(userId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(notifications.Count, result.Count);
    }
    
    [Fact]
    public async Task GetNotifications_ForUserWithNoNotifications_ShouldReturnEmptyList()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<TransactionService>>();
        var mockLocalizer = new Mock<ILocalizationService>();

        var transactionService = new TransactionService(mockUnitOfWork.Object, mockLocalizer.Object, mockLogger.Object);

        var userId = Guid.NewGuid();
        var emptyList = new List<TransactionNotificationDTO>();
        
        mockUnitOfWork.Setup(uow => uow.Transactions.GetNotificationsByUserIdAsync(userId))
            .ReturnsAsync(emptyList);
            
        // Act
        var result = await transactionService.GetUserNotificationsAsync(userId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
