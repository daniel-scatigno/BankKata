//https://katalyst.codurance.com/bank
using System.Transactions;
using Moq;
using Moq.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using BankKata.App;
using BankKata.App.Library;
using BankKata.App.Services;
using BankKata.Test.Library;
using BankKata.App.Interfaces;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BankKata.Test;

public class PrintBalance: IClassFixture<ContextProvider>
{
   private readonly ITestOutputHelper _testOutputHelper;
   private readonly IMessageSink _diagnosticMessageSink;
   private ContextProvider Context{get;set;}

   public PrintBalance(ITestOutputHelper testOutputHelper, ContextProvider context) 
   {
      _testOutputHelper = testOutputHelper;
      Context = context;
   }


   [Fact]
   public void ShouldPrintCorrectBalance()
   {
      //Capturing console output
      var consoleWriter = new ConsoleWriter();
      Console.SetOut(consoleWriter);

      //Adding an account
       Context.BankContext.Accounts.Add(new Account() { Id = 1, Name = "Daniel" });      
       Context.BankContext.SaveChanges();

      //Default account is served by the context
      var myAccount = Context.BankContext.Accounts.First();

      //Using FixedDateTimeProvider class to provide fake Date
      var timeProvider = new FixedDateTimeProvider(new DateTime(2012, 01, 10));

      //It is 10-01-2012 and Client deposit 1000
      var accountService = new AccountService(Context.BankContext, timeProvider, myAccount);
      accountService.Deposit(1000);

      //It is 13-01-2012 and Client deposit 2000
      timeProvider.SetCurrentDateTime(new DateTime(2012, 01, 13));
      accountService.Deposit(2000);

      //It is 14-01-2012 and Client withdraw 500      
      timeProvider.SetCurrentDateTime(new DateTime(2012, 01, 14));
      accountService.Withdraw(500);

      //It's TODAY and we print Bank account transactions        
      timeProvider.SetCurrentDateTime(DateTime.Now);
      accountService.PrintStatement();

      /* Checks that the console Output is
      Date || Amount || Balance
      14/01/2012 || -500 || 2500 
      13/01/2012 || 2000 || 3000 
      10/01/2012 || 1000 || 1000
      */

      Assert.Equal(consoleWriter.Outputed, "Date || Amount || Balance\n14/01/2012 || -500 || 2500 \n13/01/2012 || 2000 || 3000 \n10/01/2012 || 1000 || 1000 \n");
   }

   [Fact]
   public void ShouldBe550DolarsRicher()
   {
       Context.BankContext.Accounts.Add(new Account() { Id = 4, Name = "Gabriel" });      
       Context.BankContext.SaveChanges();

      //The default account
      var account = Context.BankContext.Accounts.First();
      int balance = account.Balance;

      //Using real time provider
      var timeProvider = new DateTimeProvider();
      var accountService = new AccountService(Context.BankContext, timeProvider, account);
      accountService.Deposit(550);

      Assert.Equal(balance+550,account.Balance);
   }


}



/* History
1) I've wrote method to implement interface, without implemmentation, just throwing NotImplementedException
2) I've wrote an AccountService inheriting from interface AccountService and wrote Account and Transaction entity class
3) I've wrote a DbContext for Data Persistence
4) On the test I've Instantiated a Mocked Entity Framework Context (In Memory Database )
5) Separated into two projects, App and Test project
5) I've implemented the AccountService, receiving an Account and Context
6) I've wrote and Interface for Providing the DateTime
7) I've implemented the DateTime provider in two different classes ,one for the fake and fixed passed Date and one for the real Now
8) I've wrote an custom TextWriter class for registering the console output and Tested successfully
9) Created a ContextProvider to serve the DbContext as a unique DbContext and a Default Account for all tests if is desired (This mean I can run all tests with the same context)
10) Created another test to verify the balance after an 550 dolares Deposit

*/