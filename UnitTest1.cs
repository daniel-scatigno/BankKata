//https://katalyst.codurance.com/bank
//https://www.thecodebuzz.com/dbcontext-mock-and-unit-test-entity-framework-net-core/
//https://stackoverflow.com/a/9911500
using System.Transactions;
using Moq;
using Moq.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
namespace BankKata;

public class UnitTest1
{

   public BankContext GetMockedDbContext()
   {
      var options = new DbContextOptionsBuilder<BankContext>()
         .UseInMemoryDatabase(databaseName: "BankDatabase")
         .Options;
      return new BankContext(options);
   }


   [Fact]
   public void ShouldPrintCorrectBalance()
   {
      var bankContext = GetMockedDbContext();
      
      //Adding a default account
      bankContext.Accounts.Add( new Account() { Id = 1, Name = "Daniel" } );
      bankContext.SaveChanges();

      var myAccount = bankContext.Accounts.First();

      //It is 10-01-2012 and Client deposit 1000
      var accountService = new AccountService(bankContext, new DateTime(2012, 01, 10), myAccount);
      accountService.Deposit(1000);

      //It is 13-01-2012 and Client deposit 2000
      accountService = new AccountService(bankContext, new DateTime(2012, 01, 13), myAccount);
      accountService.Deposit(2000);

      //It is 14-01-2012 and Client withdraw 500
      accountService = new AccountService(bankContext, new DateTime(2012, 01, 14), myAccount);
      accountService.Withdraw(500);

      //It's TODAY and we print Bank account transactions
      accountService = new AccountService(bankContext, DateTime.Today, myAccount);
      accountService.PrintStatement();
      string line = Console.ReadLine();

      Assert.Equal("bank", line);


   }
}

public class AccountService : IAccountService
{
   private BankContext BankContext { get; set; }
   private DateTime Date { get; set; }
   private Account Account { get; set; }

   public AccountService(BankContext context, DateTime date, Account account)
   {
      this.BankContext = context;
      this.Date = date;
      this.Account = account;

      //FAKE the clock, search how to do it 
      //Fake the console, I've already search, there is a way using XUnit, you receive an Interface 

   }

   public void Deposit(int amount)
   {
      this.Account.Balance +=amount;
      AccountTransaction t = new AccountTransaction()
      {
         Account = this.Account,
         Amount = amount,
         Date = this.Date,
         Type = OperationType.Deposit,
         HistoricBalance =this.Account.Balance
      };
      
      BankContext.Add(t);      
      BankContext.Update(this.Account);
      BankContext.SaveChanges();
   }

   public void Withdraw(int amount)
   {
      this.Account.Balance -=amount;
      AccountTransaction t = new AccountTransaction()
      {
         Account = this.Account,
         Amount = amount,
         Date = this.Date,
         Type = OperationType.Withdraw,
         HistoricBalance =this.Account.Balance 
      };

      BankContext.Add(t);
      BankContext.Update(this.Account);
      BankContext.SaveChanges();
   }
   public void PrintStatement()
   {     
      Console.WriteLine("Date || Amount || Balance"); 
      var transactions = BankContext.Transactions.OrderByDescending(x=>x.Date).ToList();
      int total = transactions.Where(x=>x.Type==OperationType.Deposit).Sum(x=>x.Amount) - transactions.Where(x=>x.Type==OperationType.Withdraw).Sum(x=>x.Amount);

      foreach(var t in transactions)
      {
         Console.WriteLine($"{t.Date.ToString("dd/MM/yyyy")} || {(t.Type==OperationType.Withdraw?"-":"")}{t.Amount} || {t.HistoricBalance} "); 
      }

      
      
   }
}


/* History
1) I've wrote method to implement interface, without implemmentation, just throwing NotImplementedException

*/