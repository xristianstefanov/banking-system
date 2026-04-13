using System;
using System.Collections.Generic;
namespace BankingSystemHris.Exceptions
{
    public class InvalidAmountException : Exception
    {
        public InvalidAmountException() : base("Amount must be positive.") { }
    }

    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException() : base("Insufficient balance.") { }
    }

    public class OverdraftLimitExceededException : Exception
    {
        public OverdraftLimitExceededException() : base("Overdraft limit exceeded.") { }
    }

    public class DailyLimitExceededException : Exception
    {
        public DailyLimitExceededException() : base("Daily withdrawal limit exceeded.") { }
    }

    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException() : base("Account not found.") { }
    }
}
