using System.ComponentModel.DataAnnotations;

namespace Task1Bank.Entities;

public class LanguageRequest
{
    [Required]
    public int Id { get; set; }
    
    public string Language { get; set; }
}