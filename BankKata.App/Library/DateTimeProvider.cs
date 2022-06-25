using BankKata.App.Interfaces;

namespace BankKata.App.Library
{
   public class DateTimeProvider : IDateTimeProvider
   {
      public DateTime GetNow() => DateTime.Now;
   }
}