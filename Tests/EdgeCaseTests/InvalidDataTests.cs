
using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Task1Bank.Entities;
using Task1Bank.Services;
using Task1Bank.Tests.TestHelpers;

namespace Task1Bank.Tests.EdgeCaseTests
{
    public class InvalidDataTests
    {
        [Fact]
        public async Task CreateAccount_WithEmptyAccountNumber_ShouldThrowException()
        {
            // Arrange
            var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
            var accountService = new AccountService(mockUnitOfWork.Object);
            
            var invalidAccount = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "",  
                Balance = 1000,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow,
                IsActive = true
            };
            
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await accountService.CreateAccountAsync(invalidAccount));
                
            mockUnitOfWork.Verify(uow => uow.Accounts.AddAsync(It.IsAny<Account>()), Times.Never);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
        }
        
        [Fact]
        public async Task Deposit_WithZeroAmount_ShouldThrowException()
        {
            // Arrange
            var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
            var bankingService = new BankingService(mockUnitOfWork.Object);
            
            var account = TestDataGenerator.GenerateValidAccount();
            decimal depositAmount = 0;
            
            mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
                .ReturnsAsync(account);
                
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await bankingService.DepositAsync(account.Id, depositAmount, "Test deposit"));
                
            mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()), Times.Never);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
        }
        
        [Fact]
        public async Task Withdraw_WithZeroAmount_ShouldThrowException()
        {
            // Arrange
            var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
            var bankingService = new BankingService(mockUnitOfWork.Object);
            
            var account = TestDataGenerator.GenerateValidAccount();
            decimal withdrawalAmount = 0;
            
            mockUnitOfWork.Setup(uow => uow.Accounts.GetByIdAsync(account.Id))
                .ReturnsAsync(account);
                
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await bankingService.WithdrawAsync(account.Id, withdrawalAmount, "Test withdrawal"));
                
            mockUnitOfWork.Verify(uow => uow.Transactions.AddAsync(It.IsAny<AccountTransaction>()), Times.Never);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
        }
        
        [Fact]
        public async Task CreateRecurrentTransaction_WithPastExecutionDate_ShouldThrowException()
        {
            // Arrange
            var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
            var transactionService = new TransactionService(mockUnitOfWork.Object);
            
            var account = TestDataGenerator.GenerateValidAccount();
            var invalidRecurrentTransaction = new RecurrentTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                Amount = 100,
                TransactionType = "Withdrawal",
                Frequency = "Monthly",
                NextExecutionDate = DateTime.UtcNow.AddDays(-1), 
                Description = "Monthly subscription payment"
            };
            
            mockUnitOfWork.Setup(uow => uow.AccountsRepository.GetByIdAsync(account.Id))
                .ReturnsAsync(account);
                
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionService.CreateRecurrentTransactionAsync(invalidRecurrentTransaction));
                
            mockUnitOfWork.Verify(uow => uow.TransactionRepository.AddRecurrentTransactionAsync(It.IsAny<RecurrentTransaction>()), Times.Never);
            mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }
        
        [Fact]
        public async Task CreateNotification_WithEmptyMessage_ShouldThrowException()
        {
            // Arrange
            var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
            var transactionService = new TransactionService(mockUnitOfWork.Object);
            
            var invalidNotification = new TransactionNotificationDTO
            {
                Id = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                Message = "",  // Empty message
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionService.CreateNotificationAsync(invalidNotification));
                
            mockUnitOfWork.Verify(uow => uow.TransactionRepository.CreateNotificationAsync(It.IsAny<TransactionNotificationDTO>()), Times.Never);
            mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }
    }
}