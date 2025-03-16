using Microsoft.EntityFrameworkCore;
using Task1Bank.Data;
using Task1Bank.Entities;
using Task1Bank.Entities.DTOs;

namespace Task1Bank.Repositories
{
    public class TransactionRepository : Repository<AccountTransaction>, ITransactionRepository
    {
        public TransactionRepository(BankDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AccountTransaction>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Set<AccountTransaction>()
                .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId)
                .ToListAsync();
        }

        public async Task<TransactionNotificationDTO> CreateNotificationAsync(TransactionNotificationDTO transaction)
        {
            if (transaction != null)
            {
                var MyTransaction = await _context.Set<AccountTransaction>()
                    .FirstOrDefaultAsync(t => t.TransactionId == transaction.TransactionId);
            }
            else
            {
                throw new ArgumentException("Transaction not found");

            }

            var notification = new TransactionNotificationDTO
            {
                Title = "Transaction Notification",
                TransactionTypeLabel = "Transaction Type:",
                TransactionType = transaction.TransactionType.ToString(), 
                TransactionDateLabel = Convert.ToDateTime("Date:"),
                Amount = transaction.Amount, 
                Description = transaction.Description
            };


            return notification;
        }

        public async Task<List<TransactionNotificationDTO>> GetNotificationsByUserIdAsync(Guid userId)
        {
            // Retrieve all transactions for the user (either as sender or receiver)
            var transactions = await _context.Set<AccountTransaction>()
                .Where(t => t.FromAccountId == userId || t.ToAccountId == userId)
                .ToListAsync();

            // Map the transactions to notifications
            var notifications = transactions.Select(t => new TransactionNotificationDTO
            {
                Title = userId.ToString(),
                Description = $"{t.TransactionType} of {t.Amount:C} completed",
                TransactionDateLabel = t.Timestamp,
                IsRead = false 
            }).ToList();

            return notifications;
        }
        
        public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Set<TransactionEvent>()
                .FirstOrDefaultAsync(n => n.TransactionId == notificationId);
            
            if (notification == null)
            {
                return false;
            }
            
            notification.isRead = true;
            _context.Update(notification);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}
