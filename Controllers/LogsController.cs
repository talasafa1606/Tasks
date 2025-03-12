using Task1Bank.Data;
using Task1Bank.Entities;

namespace Task1Bank.Controllers;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly BankDBContext _context;
    private readonly ILogger<LogsController> _logger;

    public LogsController(BankDBContext context, ILogger<LogsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetLogs(
        [FromQuery] Guid? requestId = null,
        [FromQuery] string routeUrl = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.Logs.AsQueryable();

            if (requestId.HasValue)
            {
                query = query.Where(l => l.RequestId == requestId.Value);
            }

            if (!string.IsNullOrEmpty(routeUrl))
            {
                query = query.Where(l => l.RouteURL.Contains(routeUrl));
            }

            if (startDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(l => l.Timestamp <= endDate.Value);
            }

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Logs = logs
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving logs");
            return StatusCode(500, "An error occurred while retrieving logs");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateLog([FromBody] LogMessage logMessage)
    {
        if (logMessage == null)
        {
            return BadRequest("Log message is required");
        }

        try
        {
            var log = new Log
            {
                RequestId = logMessage.RequestId,
                RequestObject = logMessage.RequestObject,
                RouteURL = logMessage.RouteURL,
                Timestamp = logMessage.Timestamp
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLogs), new { requestId = log.RequestId }, log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating log");
            return StatusCode(500, "An error occurred while creating the log");
        }
    }
}