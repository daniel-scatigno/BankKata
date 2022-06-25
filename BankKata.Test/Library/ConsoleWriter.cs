
using BankKata.App;
using BankKata.App.Interfaces;
using Xunit.Abstractions;
using System.Text.Encodings;
using System.Text;
namespace BankKata.Test.Library
{

   public class ConsoleWriter : TextWriter
   {
      public string Outputed{get;set;}
      private StreamWriter StandardOutput{get;set;}
      public ConsoleWriter()
      {
         StandardOutput = new StreamWriter(Console.OpenStandardOutput());
         StandardOutput.AutoFlush=true;
      }
      public override Encoding Encoding
      {
         get { return Encoding.UTF8; }
      }
      
      public override void WriteLine(string message)
      {
         Outputed+=message+"\n";
         StandardOutput.WriteLine(message);         
      }
      public override void WriteLine(string format, params object[] args)
      {
         StandardOutput.WriteLine(format,args);
         Outputed+=string.Format(format,args)+"\n";;         
      }

      public override void Write(char value)
      {
         throw new NotSupportedException("This text writer only supports WriteLine(string) and WriteLine(string, params object[]).");
      }
   }
}