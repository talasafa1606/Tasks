namespace Task1Bank.Tests.EventSourcingTests;
using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EventRollbackTests
{
    [Fact]
    public async Task RollbackEvents_ForSpecificDay_ShouldRevertEvents()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var eventService = new EventService(mockUnitOfWork.Object);
        
        var account = TestDataGenerator.GenerateValidAccount();
        var rollbackDate = DateTime.UtcNow.Date;
        
        var events = new List<TransactionEvent>
        {
            new TransactionEvent
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                EventType = "Deposit",
                Amount = 100,
                EventDate = rollbackDate,
                IsReverted = false
            },
            new TransactionEvent
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                EventType = "Withdrawal",
                Amount = 50,
                EventDate = rollbackDate,
                IsReverted = false
            }
        };
        
        mockUnitOfWork.Setup(uow => uow.EventRepository.GetEventsByDateAsync(rollbackDate))
            .ReturnsAsync(events);
            
        mockUnitOfWork.Setup(uow => uow.AccountsRepository.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        // Act
        var result = await eventService.RollbackEventsByDateAsync(rollbackDate);
        
        // Assert
        Assert.Equal(2, result); // Should revert 2 events
        mockUnitOfWork.Verify(uow => uow.EventRepository.GetEventsByDateAsync(rollbackDate), Times.Once);
        mockUnitOfWork.Verify(uow => uow.EventRepository.MarkEventAsRevertedAsync(It.IsAny<Guid>()), Times.Exactly(2));
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task RollbackEvents_ForSpecificAccount_ShouldRevertEvents()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var eventService = new EventService(mockUnitOfWork.Object);
        
        var account = TestDataGenerator.GenerateValidAccount();
        
        var events = new List<TransactionEvent>
        {
            new TransactionEvent
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                EventType = "Deposit",
                Amount = 100,
                EventDate = DateTime.UtcNow.AddDays(-1),
                IsReverted = false
            },
            new TransactionEvent
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                EventType = "Withdrawal",
                Amount = 50,
                EventDate = DateTime.UtcNow,
                IsReverted = false
            }
        };
        
        mockUnitOfWork.Setup(uow => uow.EventRepository.GetEventsByAccountIdAsync(account.Id))
            .ReturnsAsync(events);
            
        mockUnitOfWork.Setup(uow => uow.AccountsRepository.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        // Act
        var result = await eventService.RollbackEventsByAccountIdAsync(account.Id);
        
        // Assert
        Assert.Equal(2, result); // Should revert 2 events
        mockUnitOfWork.Verify(uow => uow.EventRepository.GetEventsByAccountIdAsync(account.Id), Times.Once);
        mockUnitOfWork.Verify(uow => uow.EventRepository.MarkEventAsRevertedAsync(It.IsAny<Guid>()), Times.Exactly(2));
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.AtLeastOnce);
    }
}
