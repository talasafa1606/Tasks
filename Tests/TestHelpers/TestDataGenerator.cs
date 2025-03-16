namespace Task1Bank.Tests.TestHelpers;

using Task1Bank.Entities;
using System;
using System.Collections.Generic;

public static class TestDataGenerator
{
    public static Account GenerateValidAccount()
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = "ACC" + DateTime.Now.Ticks.ToString().Substring(0, 10),
            Balance = 1000,
            CreatedDate = DateTime.UtcNow,
            LastUpdatedDate = DateTime.UtcNow,
            IsActive = true
        };
    }
    
    public static List<Account> GenerateAccountList(int count = 5)
    {
        var accounts = new List<Account>();
        for (int i = 0; i < count; i++)
        {
            accounts.Add(GenerateValidAccount());
        }
        return accounts;
    }
    
    public static AccountTransaction GenerateDeposit(Guid accountId, decimal amount = 100)
    {
        return new AccountTransaction
        {
            TransactionId = Guid.NewGuid(),
            ToAccountId = accountId,
            Amount = amount,
            TransactionType = "Deposit",
            Timestamp = DateTime.UtcNow,
            Details = "Test deposit"
        };
    }
    
    public static AccountTransaction GenerateWithdrawal(Guid accountId, decimal amount = 50)
    {
        return new AccountTransaction
        {
            TransactionId = Guid.NewGuid(),
            ToAccountId = accountId,
            Amount = amount,
            TransactionType = "Withdrawal",
            Timestamp = DateTime.UtcNow,
            Details = "Test withdrawal"
        };
    }
}
