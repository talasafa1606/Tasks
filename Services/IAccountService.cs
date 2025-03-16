using Task1Bank.Entities.DTOs;

namespace Task1Bank.Services;

public interface IAccountService
{
    Task<AccountDetailsDTO> GetAccountDetailsAsync(int accountId, string language);
    Task<AccountDetailsDTO> CreateAccountAsync(CreateAccountDTO createAccountDto);
}
