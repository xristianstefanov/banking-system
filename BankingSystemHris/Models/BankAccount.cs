namespace BankingSystemHris.Models
{
    using BankingSystemHris.Exceptions;
    using BankingSystemHris.Services;
    using System;
    using System.Collections.Generic;

    public abstract class BankAccount
    {
        private decimal balance;
        private readonly object balanceLock = new();
        private readonly List<Transaction> transactions = new();

        private decimal withdrawnToday;
        private DateTime lastWithdrawDate = DateTime.MinValue;

        protected BankAccount(string owner, string accountNumber)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
        }

        public string Owner { get; }
        public string AccountNumber { get; }

        internal Bank Bank { get; set; }

        public decimal DailyWithdrawalLimit { get; set; } = decimal.MaxValue;

        public IReadOnlyCollection<Transaction> Transactions => transactions.AsReadOnly();

        public decimal GetBalance()
        {
            lock (balanceLock)
            {
                return balance;
            }
        }

        internal virtual void Deposit(decimal amount)
        {
            ValidateAmount(amount);

            lock (balanceLock)
            {
                balance += amount;
                AddTransaction(TransactionType.Deposit, amount, to: AccountNumber);
            }
        }

        internal virtual void Withdraw(decimal amount)
        {
            ValidateAmount(amount);
            CheckDailyLimit(amount);

            lock (balanceLock)
            {
                if (!CanWithdraw(amount))
                    throw new InsufficientFundsException();

                balance -= amount;
                withdrawnToday += amount;

                AddTransaction(TransactionType.Withdraw, amount, from: AccountNumber);
            }
        }

        internal void WithdrawInternal(decimal amount)
        {
            ValidateAmount(amount);
            CheckDailyLimit(amount);

            lock (balanceLock)
            {
                if (!CanWithdraw(amount))
                    throw new InsufficientFundsException();

                balance -= amount;
                withdrawnToday += amount;
            }
        }

        internal void DepositInternal(decimal amount)
        {
            ValidateAmount(amount);

            lock (balanceLock)
            {
                balance += amount;
            }
        }

        protected virtual bool CanWithdraw(decimal amount)
        {
            return GetBalance() >= amount;
        }

        internal void ApplyFee(decimal fee)
        {
            if (fee <= 0) return;

            lock (balanceLock)
            {
                balance -= fee;
                AddTransaction(TransactionType.Fee, fee, from: AccountNumber);
            }
        }

        internal void RecordTransfer(decimal amount, string from, string to)
        {
            AddTransaction(TransactionType.Transfer, amount, from, to);
        }

        protected void AddTransaction(TransactionType type, decimal amount, string from = null, string to = null)
        {
            transactions.Add(new Transaction(type, amount, GetBalance(), from, to));
        }

        private void CheckDailyLimit(decimal amount)
        {
            if (DateTime.UtcNow.Date != lastWithdrawDate)
            {
                withdrawnToday = 0;
                lastWithdrawDate = DateTime.UtcNow.Date;
            }

            if (withdrawnToday + amount > DailyWithdrawalLimit)
            {
                throw new DailyLimitExceededException();
            }
        }

        protected void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidAmountException();
            }
        }
    }
}
