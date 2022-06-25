
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using BankKata.App.Interfaces;
using BankKata.App;
using BankKata.App.Library;

namespace BankKata.App.Services;

public class AccountService : IAccountService
{
   private BankContext BankContext { get; set; }   
   private Account Account { get; set; }
   private IDateTimeProvider TimeProvider{get;set;}

   //Time provider is a injected interface
   public AccountService(BankContext context,IDateTimeProvider timeProvider, Account account)
   {
      this.BankContext = context;      
      this.Account = account;
      TimeProvider = timeProvider;
   }
   

   public void Deposit(int amount)
   {
      this.Account.Balance +=amount;
      AccountTransaction t = new AccountTransaction()
      {
         Account = this.Account,
         Amount = amount,
         Date = TimeProvider.GetNow(),  //This Date is set by the unit testing
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
         Date = TimeProvider.GetNow(),
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
