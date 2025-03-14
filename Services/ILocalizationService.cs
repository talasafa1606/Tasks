using Task1Bank.Entities;

namespace Task1Bank.Services;

public interface ILocalizationService
{
    string GetLocalizedString(string key, string language = "en");
    string GetLocalizedString(string key, string language, params object[] args);
    string GetLocalizedTransactionDescription(AccountTransaction transaction, string language);
}