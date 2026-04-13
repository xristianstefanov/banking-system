using BankingSystemHris.Models;
using BankingSystemHris.Services;

class Program
{
    static void Main()
    {
        var bank = new Bank();

        var s = bank.CreateSavings("Ivan", 0.05m, 5000);
        var c = bank.CreateChecking("Ivan", 200);

        bank.Deposit(s.AccountNumber, 1000);
        bank.Deposit(c.AccountNumber, 500);

        bank.Transfer(s.AccountNumber, c.AccountNumber, 200);
        bank.Withdraw(c.AccountNumber, 600);

        s.ApplyInterest(CompoundingMode.Monthly);

        foreach (var t in bank.Statement(c.AccountNumber, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow))
        {
            Console.WriteLine($"{t.Type} | {t.Amount} | {t.ResultingBalance}");
        }
    }
}