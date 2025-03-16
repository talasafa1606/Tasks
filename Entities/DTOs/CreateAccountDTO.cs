namespace Task1Bank.Entities.DTOs;
public class CreateAccountDTO
{
    public string AccountNumber { get; set; }
    public string CustomerName { get; set; }
    public decimal InitialDeposit { get; set; }
    public string PreferredLanguage { get; set; } = "en";
}