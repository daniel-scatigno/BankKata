using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BankKata
{
    public class BankContext : DbContext
    {
        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AccountTransaction> Transactions { get; set; } = null!;
    }

    public class Account
    {
        public int Id { get; set; }
        public string? Name { get; set; }        

        public virtual List<AccountTransaction> Transactions { get; set; } = new ();
    }

    public class AccountTransaction
    {
        public int Id{get;set;}
        public DateTime Date { get; set; }
        public OperationType Type{get;set;}

        public int Amount{get;set;}

        public Account Account{get;set;} = new();

    }

    public enum OperationType{Deposit,Withdraw};
}