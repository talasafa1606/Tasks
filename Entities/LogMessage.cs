namespace Task1Bank.Entities;
using System;

public class LogMessage
{
    public Guid RequestId { get; set; }
    public string RequestObject { get; set; }
    public string RouteURL { get; set; }
    public DateTime Timestamp { get; set; }
}