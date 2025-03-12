using Task1Bank.Data;
using Task1Bank.Entities;

namespace Task1Bank.Controllers;

using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class TransactionLogsController : ControllerBase
{
    private readonly BankDBContext _context;
    private readonly ILogger<TransactionLogsController> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public TransactionLogsController(
        BankDBContext context, 
        ILogger<TransactionLogsController> logger,
        IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransactionLog([FromBody] TransactionLogRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var transactionLog = new TransactionLog
            {
                AccountId = request.AccountId,
                TransactionType = request.TransactionType,
                Amount = request.Amount,
                Status = request.Status,
                Details = request.Details,
                Timestamp = DateTime.UtcNow
            };

            _context.TransactionLogs.Add(transactionLog);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new TransactionLogMessage
            {
                Id = transactionLog.Id,
                AccountId = transactionLog.AccountId,
                TransactionType = transactionLog.TransactionType,
                Amount = transactionLog.Amount,
                Timestamp = transactionLog.Timestamp,
                Status = transactionLog.Status,
                Details = transactionLog.Details
            });

            _logger.LogInformation($"Transaction log created and published for account {request.AccountId}.");
            
            return CreatedAtAction(
                nameof(GetTransactionLogsByAccountId), 
                new { accountId = request.AccountId }, 
                transactionLog
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create transaction log.");
            return StatusCode(500, "Something went wrong while processing your request.");
        }
    }

    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetTransactionLogsByAccountId(long accountId)
    {
        try
        {
            var logs = await _context.TransactionLogs
                .Where(t => t.AccountId == accountId)
                .ToListAsync();
                
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving transaction logs for account with the id of {accountId}");
            return StatusCode(500, "Something went wrong while processing your request.");
        }
    }

    [HttpGet]
    [EnableQuery]
    public IActionResult GetTransactionLogs()
    {
        try
        {
            return Ok(_context.TransactionLogs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction logs");
            return StatusCode(500, "Something went wrong while processing your request.");
        }
    }
}
