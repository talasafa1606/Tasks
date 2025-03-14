using System.Globalization;
using Microsoft.Extensions.Localization;
using Task1Bank.Entities;

namespace Task1Bank.Services;

public class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer<Resources> _localizer;
    private readonly ILogger<LocalizationService> _logger;
    
    public LocalizationService(IStringLocalizer<Resources> localizer, ILogger<LocalizationService> logger)
    {
        _localizer = localizer;
        _logger = logger;
    }
    
    public string GetLocalizedString(string key, string language = "en")
    {
        SetCulture(language);
        
        return _localizer[key];
    }
    
    public string GetLocalizedString(string key, string language, params object[] args)
    {
        SetCulture(language);
        return string.Format(_localizer[key], args);
    }
    
   
    public string GetLocalizedTransactionDescription(AccountTransaction transaction, string language)
    {
        if (transaction == null)
            return string.Empty;
            
        string localizedDescription = language switch
        {
            "fr" => !string.IsNullOrEmpty(transaction.DescriptionFr) ? transaction.DescriptionFr : transaction.Details,
            "de" => !string.IsNullOrEmpty(transaction.DescriptionDe) ? transaction.DescriptionDe : transaction.Details,
            _ => transaction.Details
        };
        
        if (string.IsNullOrEmpty(localizedDescription))
        {
            SetCulture(language);
            
            return transaction.TransactionType switch
            {
                "Deposit" => GetLocalizedString("DepositDescription", language, transaction.ToAccount?.AccountNumber),
                "Withdrawal" => GetLocalizedString("WithdrawalDescription", language, transaction.FromAccount?.AccountNumber),
                "Transfer" => GetLocalizedString("TransferDescription", language, transaction.FromAccount?.AccountNumber, transaction.ToAccount?.AccountNumber),
                _ => string.Empty
            };
        }
        
        return localizedDescription;
    }
    
    private void SetCulture(string language)
    {
        var culture = language switch
        {
            "fr" => new CultureInfo("fr-FR"),
            "de" => new CultureInfo("de-DE"),
            _ => new CultureInfo("en-US")
        };
        
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
    }
}