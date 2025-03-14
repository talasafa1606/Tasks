namespace Task1Bank.Entities;

public class Account
{
    public int Id { get; set; }
    public string AccountNumber { get; set; }
    public string CustomerName { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    
    public ICollection<AccountTransaction> Transactions { get; set; }
}