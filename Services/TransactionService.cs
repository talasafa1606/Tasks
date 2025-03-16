using System.Globalization;
using Task1Bank.Data;
using Task1Bank.Entities;
using Task1Bank.Entities.DTOs;
using Task1Bank.UOF;

namespace Task1Bank.Services;

public class TransactionService : ITransactionService
{
    private readonly BankDBContext _context;
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
    
    public async Task<TransactionNotificationDTO> GetTransactionNotificationAsync(Guid transactionId, string language)
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
            TransactionDate = transaction.Timestamp.ToString("g", cultureInfo),
            Amount = transaction.Amount.ToString("C", cultureInfo),
            Description = _localizationService.GetLocalizedTransactionDescription(transaction, language)
        };
    }
  
    public async Task<bool> CreateNotificationAsync(TransactionNotificationDTO notification)
    {
        try
        {
            var transactionNotification = new TransactionLog()
            {
                TransactionType = notification.TransactionType,
                Timestamp = notification.TransactionDateLabel,
                Amount = notification.AmountLabel,
                Status = notification.Amount,
                Details = notification.Description,
            };

            _context.TransactionLogs.Add(transactionNotification);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Log the error
            _logger.LogError($"Error creating notification: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> CreateTransactionNotificationAsync(AccountTransaction transaction)
    {
        try
        {
            // Check if the transaction is valid
            if (transaction == null)
            {
                _logger.LogWarning("Transaction is null");
                return false;
            }

            // Create notification DTO based on the transaction data
            var language = "en-US"; // Default language, you can retrieve this dynamically if needed
            var transactionNotification = await GetTransactionNotificationAsync(transaction.TransactionId, language);

            if (transactionNotification == null)
            {
                _logger.LogWarning("Transaction notification could not be created. Transaction may be invalid.");
                return false;
            }

            // Log the transaction details if needed
            _logger.LogInformation($"Creating notification for transaction {transaction.TransactionId} - {transaction.Amount}");

            // Create and save the notification
            var notificationCreated = await _unitOfWork.Transactions.CreateNotificationAsync(transactionNotification);

            if (notificationCreated != null)
            {

                _logger.LogError($"Failed to create notification for transaction {transaction.TransactionId}");
                return false;
            }
            else
            {
                return true;}
            
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating transaction notification for transaction {transaction.TransactionId}: {ex.Message}");
            return false;
        }
    }
    
   

    public async Task<bool> MarkNotificationAsReadAsync(Guid invalidNotificationId)
    {
        _logger.LogInformation($"Retrieving notifications for user {invalidNotificationId}");

        // Fetch notifications from the repository
        var notifications = await _unitOfWork.Transactions.GetNotificationsByUserIdAsync(invalidNotificationId);
    
        // If no notifications are found, log and return an empty list
        if (notifications == null || notifications.Count == 0)
        {
            _logger.LogWarning($"No notifications found for user {invalidNotificationId}");
            return new List<TransactionNotificationDTO>();
        }

        return notifications;    }
}