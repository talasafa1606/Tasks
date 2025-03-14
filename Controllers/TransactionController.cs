using Microsoft.AspNetCore.Mvc;
using Task1Bank.Entities;
using Task1Bank.Entities.DTOs;
using Task1Bank.Services;

namespace Task1Bank.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionsController> _logger;
    
    public TransactionsController(
        ITransactionService transactionService,
        ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }
    
    [HttpPost("notify")]
    public async Task<ActionResult<TransactionNotificationDTO>> NotifyTransaction([FromBody] LanguageRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        string language = !string.IsNullOrEmpty(request.Language) ? 
            request.Language : 
            HttpContext.Items["Language"]?.ToString() ?? "en";
            
        if (!new[] { "en", "fr", "de" }.Contains(language))
        {
            return BadRequest(new { Error = "Unsupported language. Supported languages are: English (en), French (fr), German (de)" });
        }
        
        var notification = await _transactionService.GetTransactionNotificationAsync(request.Id, language);
        
        if (notification == null)
            return NotFound();
            
        return Ok(notification);
    }
}