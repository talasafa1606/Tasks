namespace Task1Bank.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TransactionEvent
{
    [Key]
    public long Id { get; set; }
    
    public Guid TransactionId { get; set; }
    
    public string EventType { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string Details { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    public int Version { get; set; }
    public bool isRead { get; set; }
}