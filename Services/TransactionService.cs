using System.Globalization;
using Task1Bank.Entities.DTOs;
using Task1Bank.UOF;

namespace Task1Bank.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger<TransactionService> _logger;
    
    public TransactionService(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService,
        ILogger<TransactionService> logger)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
        _logger = logger;
    }
    
    public async Task<TransactionNotificationDTO> GetTransactionNotificationAsync(int transactionId, string language)
    {
        _logger.LogInformation($"Generating transaction notification for transaction {transactionId} in language {language}");
        
        var transaction = await _unitOfWork.Transactions.GetByIdAsync(transactionId);
        if (transaction == null)
        {
            _logger.LogWarning($"Transaction {transactionId} not found");
            return null;
        }
        
        if (transaction.FromAccountId.HasValue && transaction.FromAccount == null)
        {
            transaction.FromAccount = await _unitOfWork.Accounts.GetByIdAsync(transaction.FromAccountId.Value);
        }
        
        if (transaction.ToAccountId.HasValue && transaction.ToAccount == null)
        {
            transaction.ToAccount = await _unitOfWork.Accounts.GetByIdAsync(transaction.ToAccountId.Value);
        }
        
        // Create culture info for formatting based on language
        var cultureInfo = language switch 
        {
            "fr" => new CultureInfo("fr-FR"), 
            "de" => new CultureInfo("de-DE"), 
            _ => new CultureInfo("en-US")
        };
        
        return new TransactionNotificationDTO
        {
            Title = _localizationService.GetLocalizedString("TransactionNotification", language),
            TransactionType = _localizationService.GetLocalizedString("TransactionType", language),
            TransactionDateLabel = _localizationService.GetLocalizedString("TransactionDate", language),
            TransactionDate = transaction.Timestamp.ToString("g", cultureInfo),
            AmountLabel = _localizationService.GetLocalizedString("Amount", language),
            Amount = transaction.Amount.ToString("C", cultureInfo),
            Description = _localizationService.GetLocalizedTransactionDescription(transaction, language)
        };
    }
}