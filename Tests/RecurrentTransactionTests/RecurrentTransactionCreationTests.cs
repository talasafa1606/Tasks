using Task1Bank.Services;
using Task1Bank.Tests.TestHelpers;

namespace Task1Bank.Tests.RecurrentTransactionTests;

using Xunit;
using Moq;
using System;
using System.Threading.Tasks;

public class RecurrentTransactionCreationTests
{
    [Fact]
    public async Task CreateRecurrentTransaction_WithValidData_ShouldCreateRecurrentTransaction()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<TransactionService>>();
        var mockLocalizer = new Mock<ILocalizationService>();

        var transactionService = new TransactionService(mockUnitOfWork.Object, mockLocalizer.Object, mockLogger.Object);
 
        var account = TestDataGenerator.GenerateValidAccount();
        var recurrentTransaction = new RecurrentTransaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = 100,
            TransactionType = "Withdrawal",
            Frequency = "Monthly",
            NextExecutionDate = DateTime.UtcNow.AddDays(30),
            Description = "Monthly subscription payment"
        };
        
        mockUnitOfWork.Setup(uow => uow.AccountsRepository.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
            
        mockUnitOfWork.Setup(uow => uow.TransactionRepository.AddRecurrentTransactionAsync(It.IsAny<RecurrentTransaction>()))
            .ReturnsAsync(recurrentTransaction);
            
        // Act
        var result = await transactionService.CreateRecurrentTransactionAsync(recurrentTransaction);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(recurrentTransaction.Id, result.Id);
        mockUnitOfWork.Verify(uow => uow.TransactionRepository.AddRecurrentTransactionAsync(It.IsAny<RecurrentTransaction>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateRecurrentTransaction_WithInvalidAccount_ShouldThrowException()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var transactionService = new TransactionService(mockUnitOfWork.Object);

        var invalidAccountId = Guid.NewGuid();
         var invalidAccountId = Guid.NewGuid();
            var recurrentTransaction = new RecurrentTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = invalidAccountId,
                Amount = 100,
                TransactionType = "Withdrawal",
                Frequency = "Monthly",
                NextExecutionDate = DateTime.UtcNow.AddDays(30),
                Description = "Monthly subscription payment"
            };
            
            mockUnitOfWork.Setup(uow => uow.AccountsRepository.GetByIdAsync(invalidAccountId))
                .ReturnsAsync((Account)null);
                
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await transactionService.CreateRecurrentTransactionAsync(recurrentTransaction));
                
            mockUnitOfWork.Verify(uow => uow.TransactionRepository.AddRecurrentTransactionAsync(It.IsAny<RecurrentTransaction>()), Times.Never);
            mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }
        
        [Fact]
        public async Task CreateRecurrentTransaction_WithInvalidAmount_ShouldThrowException()
        {
            // Arrange
            var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
            var transactionService = new TransactionService(mockUnitOfWork.Object);
            
            var account = TestDataGenerator.GenerateValidAccount();
            var recurrentTransaction = new RecurrentTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                Amount = -100, // Invalid negative amount
                TransactionType = "Withdrawal",
                Frequency = "Monthly",
                NextExecutionDate = DateTime.UtcNow.AddDays(30),
                Description = "Monthly subscription payment"
            };
            
            mockUnitOfWork.Setup(uow => uow.AccountsRepository.GetByIdAsync(account.Id))
                .ReturnsAsync(account);
                
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionService.CreateRecurrentTransactionAsync(recurrentTransaction));
                
            mockUnitOfWork.Verify(uow => uow.TransactionRepository.AddRecurrentTransactionAsync(It.IsAny<RecurrentTransaction>()), Times.Never);
            mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }
    }
}