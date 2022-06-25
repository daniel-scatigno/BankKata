using System.Collections.ObjectModel;
using System.Runtime.Intrinsics.X86;
using System;
using BankKata.App.Services;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
namespace BankKata.Test;

public class ContextProvider
{   
   public BankContext BankContext{get;set;}

   public ContextProvider()
   {
      var options = new DbContextOptionsBuilder<BankContext>().UseInMemoryDatabase(databaseName: "BankDatabase").Options;
       BankContext = new BankContext(options);
   }
   
}