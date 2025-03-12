using Task1Bank.Data;
using Task1Bank.Entities;

namespace Task1Bank.Consumers;

using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

public class LogConsumer : IConsumer<LogMessage>
{
    private readonly BankDBContext _context;
    private readonly ILogger<LogConsumer> _logger;

    public LogConsumer(BankDBContext context, ILogger<LogConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LogMessage> context)
    {
        try
        {
            var message = context.Message;
            
            var log = new Log
            {
                RequestId = message.RequestId,
                RequestObject = message.RequestObject,
                RouteURL = message.RouteURL,
                Timestamp = message.Timestamp
            };
            
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Saved log with RequestId: {message.RequestId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming log message");
            throw;
        }
    }
}