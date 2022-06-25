using BankKata.App;
using BankKata.App.Interfaces;

namespace BankKata.Test.Library;


public class FixedDateTimeProvider : IDateTimeProvider
{
    private DateTime _fixedDateTime;

    public FixedDateTimeProvider(DateTime fixedDateTime)
        => SetCurrentDateTime(fixedDateTime);

    public DateTime GetNow() => _fixedDateTime;

    public void SetCurrentDateTime(DateTime fixedDateTime)
    {
      _fixedDateTime = fixedDateTime;
    }
}