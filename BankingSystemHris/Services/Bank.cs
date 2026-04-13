namespace BankingSystemHris.Services
{
    using BankingSystemHris.Exceptions;
    using BankingSystemHris.Models;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class Bank
    {
        private readonly ConcurrentDictionary<string, BankAccount> accounts = new();

        public decimal WithdrawalFee { get; set; } = 1;
        public decimal OverdraftFee { get; set; } = 5;

        private string GenerateAccountNumber() => Guid.NewGuid().ToString();

        public SavingsAccount CreateSavings(string owner, decimal rate, decimal maxBalance)
        {
            var acc = new SavingsAccount(owner, GenerateAccountNumber(), rate, maxBalance);
            acc.Bank = this;
            accounts[acc.AccountNumber] = acc;
            return acc;
        }

        public CheckingAccount CreateChecking(string owner, decimal overdraftLimit)
        {
            var acc = new CheckingAccount(owner, GenerateAccountNumber(), overdraftLimit);
            acc.Bank = this;
            accounts[acc.AccountNumber] = acc;
            return acc;
        }

        public BankAccount Get(string accNum)
        {
            if (!accounts.TryGetValue(accNum, out var acc))
            {
                throw new AccountNotFoundException();
            }

            return acc;
        }

        public void Deposit(string accNum, decimal amount)
        {
            Get(accNum).Deposit(amount);
        }

        public void Withdraw(string accNum, decimal amount)
        {
            var acc = Get(accNum);

            acc.Withdraw(amount);
            acc.ApplyFee(WithdrawalFee);

            if (acc is CheckingAccount c)
            {
                c.ApplyOverdraftFee(OverdraftFee);
            }
        }

        public void Transfer(string from, string to, decimal amount)
        {
            var acc1 = Get(from);
            var acc2 = Get(to);

            var locks = new[] { acc1, acc2 }.OrderBy(a => a.AccountNumber).ToArray();

            lock (locks[0])
                lock (locks[1])
                {
                    acc1.WithdrawInternal(amount);
                    acc2.DepositInternal(amount);

                    acc1.RecordTransfer(amount, from, to);
                    acc2.RecordTransfer(amount, from, to);
                }
        }

        public IEnumerable<Transaction> Statement(string accNum, DateTime from, DateTime to)
        {
            return Get(accNum).Transactions
                .Where(t => t.Timestamp >= from && t.Timestamp <= to);
        }
    }
}
