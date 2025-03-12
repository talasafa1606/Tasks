using Task1Bank.Events;
using Task1Bank.Services;

namespace Task1Bank.Controllers;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IEventService eventService, ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }

    // POST api/events
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] TransactionDomainEventRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _eventService.CreateAndDispatchEvent(request);
            return CreatedAtAction(nameof(GetEventsByTransactionId), new { transactionId = request.TransactionId }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid event request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // GET api/events/{transactionId}
    [HttpGet("{transactionId}")]
    public async Task<IActionResult> GetEventsByTransactionId(long transactionId)
    {
        try
        {
            var events = await _eventService.GetEventsByTransactionId(transactionId);
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving events for transaction {transactionId}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}