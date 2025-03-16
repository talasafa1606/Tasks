using System.ComponentModel.DataAnnotations;

namespace Task1Bank.Entities;

public class LanguageRequest
{
    [Required]
    public Guid Id { get; set; }
    
    public string Language { get; set; }
}