using Task1Bank.Data;

namespace Task1Bank.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly BankDBContext _context;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(BankDBContext context, ILogger<AccountsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("common-transactions")]
    public async Task<IActionResult> GetCommonTransactions([FromQuery] List<long> accountIds)
    {
        if (accountIds?.Count < 2)
            return BadRequest("At least 2 account IDs must be provided.");

        try
        {
            var transactions = await _context.AccountTransactions
                .Where(t => accountIds.Contains(t.AccountId))
                .ToListAsync();

            var commonTransactions = transactions
                .GroupBy(t => (t.TransactionType, t.Amount))
                .Where(g => g.Select(t => t.AccountId).Distinct().Count() > 1) // Ensure transactions belong to multiple accounts
                .Select(g => new
                {
                    g.Key.TransactionType,
                    g.Key.Amount,
                    AccountIds = g.Select(t => t.AccountId).Distinct().ToList(),
                    Transactions = g.Select(t => new
                    {
                        t.TransactionId,
                        t.AccountId,
                        t.Timestamp,
                        t.Status,
                        t.Details
                    }).ToList()
                })
                .ToList();

            return Ok(commonTransactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving common transactions");
            return StatusCode(500, "Something went wrong while processing your request.");
        }
    }


    [HttpGet("balance-summary/{userId}")]
    public async Task<IActionResult> GetAccountBalanceSummary(long userId)
    {
        try
        {
            var userTransactions = await _context.AccountTransactions
                .Where(t => t.UserId == userId && t.Status == "Completed")
                .ToListAsync();

            var accountSummaries = userTransactions
                .GroupBy(t => t.AccountId)
                .Select(g => new
                {
                    AccountId = g.Key,
                    TotalDeposits = g.Sum(t => t.TransactionType == "Deposit" ? t.Amount : 0),
                    TotalWithdrawals = g.Sum(t => t.TransactionType == "Withdrawal" ? t.Amount : 0),
                    Balance = g.Sum(t => t.TransactionType == "Deposit" ? t.Amount : -t.Amount),
                    TransactionCount = g.Count()
                })
                .ToList();

            var overallSummary = new
            {
                UserId = userId,
                TotalAccounts = accountSummaries.Count,
                TotalDeposits = accountSummaries.Sum(a => a.TotalDeposits),
                TotalWithdrawals = accountSummaries.Sum(a => a.TotalWithdrawals),
                TotalBalance = accountSummaries.Sum(a => a.Balance),
                AccountSummaries = accountSummaries
            };

            return Ok(overallSummary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving balance summary for user {userId}");
            return StatusCode(500, "Something went wrong while processing your request.");
        }
    }
}