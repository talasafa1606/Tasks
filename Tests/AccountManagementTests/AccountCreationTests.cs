using Xunit;
using Moq;
using Task1Bank.Services;
using Task1Bank.Entities;
using Task1Bank.Entities.DTOs;
using Task1Bank.Tests.TestHelpers;

public class AccountCreationTests
{
 
    [Fact]
public async Task CreateAccount_WithValidData_ShouldCreateAccount()
{
    // Arrange
    var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
    var mockLogger = new Mock<ILogger<AccountService>>();
    var mockLocalizer = new Mock<ILocalizationService>();
    var accountService = new AccountService(mockUnitOfWork.Object, mockLocalizer.Object, mockLogger.Object);

    // Create a valid DTO input
    var createAccountDto = new CreateAccountDTO
    {
        AccountNumber = "123456789",
        CustomerName = "John Doe",
        InitialDeposit = 1000.50m,
        PreferredLanguage = "en"
    };

    // Expected Account entity after creation
    var newAccount = new Account
    {
        Id = Guid.NewGuid(),
        AccountNumber = createAccountDto.AccountNumber,
        CustomerName = createAccountDto.CustomerName,
        Balance = createAccountDto.InitialDeposit
    };

    // Expected DTO that will be returned after account creation
    var expectedAccountDetailsDto = new AccountDetailsDTO
    {
        AccountNumber = newAccount.AccountNumber,
        AccountName = newAccount.CustomerName,
        Balance = newAccount.Balance,
    };

    mockUnitOfWork.Setup(uow => uow.Accounts.AddAsync(It.IsAny<Account>()))
        .ReturnsAsync(newAccount); 
    mockUnitOfWork.Setup(uow => uow.CompleteAsync())
        .ReturnsAsync(1); // Simulate successful completion with an integer result

    // Mock GetAccountDetailsAsync method to return expected DTO
    mockUnitOfWork.Setup(uow => uow.Accounts.GetAccountDetailsAsync(It.IsAny<Guid>(), It.IsAny<string>()))
        .ReturnsAsync(expectedAccountDetailsDto); // Explicitly return AccountDetailsDTO

    // Act
    var result = await accountService.CreateAccountAsync(createAccountDto);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedAccountDetailsDto.AccountNumber, result.AccountNumber);
    Assert.Equal(expectedAccountDetailsDto.AccountName, result.AccountName);
    Assert.Equal(expectedAccountDetailsDto.Balance, result.Balance);

    // Verify repository methods
    mockUnitOfWork.Verify(uow => uow.Accounts.AddAsync(It.IsAny<Account>()), Times.Once);
    mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
}


    [Fact]
    public async Task CreateAccount_WithInvalidData_ShouldThrowException()
    {
        // Arrange
        var mockUnitOfWork = MockHelpers.GetMockUnitOfWork();
        var mockLogger = new Mock<ILogger<AccountService>>();
        var mockLocalizer = new Mock<ILocalizationService>();
        var accountService = new AccountService(mockUnitOfWork.Object, mockLocalizer.Object, mockLogger.Object);

        CreateAccountDTO invalidAccountDto = null; // Invalid input

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await accountService.CreateAccountAsync(invalidAccountDto));

        mockUnitOfWork.Verify(uow => uow.Accounts.AddAsync(It.IsAny<Account>()), Times.Never);
        mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
    }
}
