using Task1Bank.Entities;
using Task1Bank.Tests.TestHelpers;

namespace Task1Bank.Tests.EventSourcingTests;

using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


 public class EventFilteringTests
    {
        [Fact]
        public async Task FilterEvents_ByAccountAndType_ShouldReturnFilteredEvents()
        {
            // Arrange
            var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
            var eventService = new EventService(mockUnitOfWork.Object);
            
            var account = TestDataGenerator.GenerateValidAccount();
            string eventType = "Deposit";
            
            var allEvents = new List<TransactionEvent>
            {
                new TransactionEvent
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    EventType = "Deposit",
                    Amount = 100,
                    Timestamp = DateTime.UtcNow.AddDays(-1),
                    Description = "Deposit 1"
                },
                new TransactionEvent
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    EventType = "Withdrawal",
                    Amount = 50,
                    Timestamp = DateTime.UtcNow.AddDays(-1),
                    Description = "Withdrawal 1"
                },
                new TransactionEvent
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    EventType = "Deposit",
                    Amount = 200,
                    Timestamp = DateTime.UtcNow,
                    Description = "Deposit 2"
                }
            };
            
            mockUnitOfWork.Setup(uow => uow.TransactionRepository.GetEventsByAccountIdAsync(account.Id))
                .ReturnsAsync(allEvents);
                
            // Act
            var result = await eventService.GetEventsByAccountAndTypeAsync(account.Id, eventType);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.True(result.All(e => e.EventType == eventType));
            Assert.True(result.All(e => e.AccountId == account.Id));
        }
        
        [Fact]
        public async Task FilterEvents_ByDateRange_ShouldReturnEventsInRange()
        {
            // Arrange
            var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
            var eventService = new EventService(mockUnitOfWork.Object);
            
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow.AddDays(-3);
            
            var allEvents = new List<TransactionEvent>
            {
                new TransactionEvent
                {
                    Id = Guid.NewGuid(),
                    AccountId = Guid.NewGuid(),
                    EventType = "Deposit",
                    Amount = 100,
                    Timestamp = DateTime.UtcNow.AddDays(-10),
                    Description = "Old deposit"
                },
                new TransactionEvent
                {
                    Id = Guid.NewGuid(),
                    AccountId = Guid.NewGuid(),
                    EventType = "Withdrawal",
                    Amount = 50,
                    Timestamp = DateTime.UtcNow.AddDays(-5),
                    Description = "Withdrawal in range"
                },
                new TransactionEvent
                {
                    Id = Guid.NewGuid(),
                    AccountId = Guid.NewGuid(),
                    EventType = "Deposit",
                    Amount = 200,
                    Timestamp = DateTime.UtcNow.AddDays(-1),
                    Description = "Recent deposit"
                }
            };
            
            mockUnitOfWork.Setup(uow => uow.Transactions.GetEventsByDateRangeAsync(startDate, endDate))
                .ReturnsAsync(allEvents.Where(e => e.Timestamp >= startDate && e.Timestamp <= endDate).ToList());
                
            // Act
            var result = await eventService.GetEventsByDateRangeAsync(startDate, endDate);
            
            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True((bool)result.All(e => e.Timestamp >= startDate && e.Timestamp <= endDate));
        }
    }