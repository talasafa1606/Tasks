﻿using System.Globalization;
using Task1Bank.Entities;
using Task1Bank.Entities.DTOs;
using Task1Bank.UOF;

namespace Task1Bank.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger<AccountService> _logger;
    
    public AccountService(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService,
        ILogger<AccountService> logger)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
        _logger = logger;
    }
    
    public async Task<AccountDetailsDTO> GetAccountDetailsAsync(Guid accountId, string language)
    {
        _logger.LogInformation($"Retrieving account details for account {accountId} in language {language}");
        
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
        if (account == null)
        {
            _logger.LogWarning($"Account {accountId} not found");
            return null;
        }
        
        var cultureInfo = new CultureInfo(language switch 
        {
            "fr" => "fr-FR", 
            "de" => "de-DE", 
            _ => "en-US"
        });

        return new AccountDetailsDTO
        {
            Title = _localizationService.GetLocalizedString("AccountDetails", language),
            AccountNumberLabel = _localizationService.GetLocalizedString("AccountNumber", language),
            AccountNumber = account.AccountNumber,
            AccountNameLabel = _localizationService.GetLocalizedString("AccountName", language),
            BalanceLabel = _localizationService.GetLocalizedString("Balance", language),
            Balance = account.Balance,
            StatusLabel = _localizationService.GetLocalizedString("Status", language),
            Status = _localizationService.GetLocalizedString(account.IsActive ? "Active" : "Inactive", language),
            CreatedDateLabel = _localizationService.GetLocalizedString("CreatedDate", language),
            CreatedDate = account.CreatedDate.ToString("d", cultureInfo),
            LastUpdatedLabel = _localizationService.GetLocalizedString("LastUpdated", language),
            LastUpdated = account.LastUpdatedDate.ToString("d", cultureInfo)
        };
    }

    public Task<AccountDetailsDTO> GetAccountDetailsAsync(int accountId, string language)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountDetailsDTO> CreateAccountAsync(CreateAccountDTO createAccountDto)
    {
        _logger.LogInformation($"Creating a new account for {createAccountDto.CustomerName}");

        var newAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = createAccountDto.AccountNumber,
            CustomerName = createAccountDto.CustomerName,
            Balance = createAccountDto.InitialDeposit,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            LastUpdatedDate = DateTime.UtcNow,
            PreferredLanguage = createAccountDto.PreferredLanguage
        };

        await _unitOfWork.Accounts.AddAsync(newAccount);
        await _unitOfWork.CompleteAsync();

        return await GetAccountDetailsAsync(newAccount.Id, createAccountDto.PreferredLanguage);
    }
}
