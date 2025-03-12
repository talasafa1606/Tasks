namespace Task1Bank.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class Log
{
    [Key]
    public long Id { get; set; }
    
    public Guid RequestId { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string RequestObject { get; set; }
    
    public string RouteURL { get; set; }
    
    public DateTime Timestamp { get; set; }
}