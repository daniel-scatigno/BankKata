using System.Transactions;
//https://katalyst.codurance.com/bank

using Moq;
using Moq.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace BankKata;

public class UnitTest1
{
   [Fact]
   public void ShouldPrintCorrectBalance()
   {
      var bankContextMoq = new Mock<BankContext>();
      List<Account> accounts = new() { new Account() { Id = 1, Name = "Daniel" } };
      List<AccountTransaction> transactions = new();
      bankContextMoq.Setup(x => x.Accounts).ReturnsDbSet(accounts);
      bankContextMoq.Setup(x => x.Transactions).ReturnsDbSet(transactions);
      bankContextMoq.Setup(x => x.Add(new AccountTransaction())).Returns((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AccountTransaction>) null);
      var bankContext = bankContextMoq.Object;

      var myAccount = bankContext.Accounts.First();

      //It is 10-01-2012 and Client deposit 1000
      var accountService = new AccountService(bankContext, new DateTime(2012, 01, 10), myAccount);
      accountService.Deposit(1000);

      //It is 13-01-2012 and Client deposit 2000
      accountService = new AccountService(bankContext, new DateTime(2012, 01, 10), myAccount);
      accountService.Deposit(2000);

      //It is 14-01-2012 and Client withdraw 500
      accountService = new AccountService(bankContext, new DateTime(2012, 01, 10), myAccount);
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

   }

   public void Deposit(int amount)
   {
      AccountTransaction t = new AccountTransaction()
      {
         Account = this.Account,
         Amount = amount,
         Date = this.Date,
         Type = OperationType.Deposit,
         Id=1
      };
      BankContext.Add(t);      
      BankContext.SaveChanges();
   }

   public void Withdraw(int amount)
   {
      AccountTransaction t = new AccountTransaction()
      {
         Account = this.Account,
         Amount = amount,
         Date = this.Date,
         Type = OperationType.Withdraw,
         Id=3
      };

      BankContext.Add(t);
      BankContext.SaveChanges();
   }
   public void PrintStatement()
   {     
      Console.WriteLine("Date || Amount || Balance"); 
      var transactions = BankContext.Transactions.OrderByDescending(x=>x.Date).ToList();
      int total = transactions.Where(x=>x.Type==OperationType.Deposit).Sum(x=>x.Amount) - transactions.Where(x=>x.Type==OperationType.Withdraw).Sum(x=>x.Amount);

      foreach(var t in transactions)
      {
         Console.WriteLine($"{t.Date.ToString("dd/MM/yyyy")} || {t.Amount} || "); 
      }
      
   }
}


/* History
1) I've wrote method to implement interface, without implemmentation, just throwing NotImplementedException

*/