using Task1Bank.Data;
using Task1Bank.Repositories;
using Task1Bank.UOF;

namespace Task1Bank.Tests.TestHelpers;

using Moq;
using Microsoft.EntityFrameworkCore;
using System;

public static class MockHelpers
{
    public static BankDBContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<BankDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        return new BankDBContext(options);
    }
    
    public static Mock<IAccountRepository> GetMockAccountRepository()
    {
        return new Mock<IAccountRepository>();
    }
    
    public static Mock<ITransactionRepository> GetMockTransactionRepository()
    {
        return new Mock<ITransactionRepository>();
    }
    
    public static Mock<IUnitOfWork> GetMockUnitOfWork()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        
        var mockAccountRepo = GetMockAccountRepository();
        var mockTransactionRepo = GetMockTransactionRepository();
        
        mockUnitOfWork.Setup(uow => uow.Accounts).Returns(mockAccountRepo.Object);
        mockUnitOfWork.Setup(uow => uow.Transactions).Returns(mockTransactionRepo.Object);
        
        return mockUnitOfWork;
    }
}
