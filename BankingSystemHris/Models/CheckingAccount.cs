namespace BankingSystemHris.Models
{
    using BankingSystemHris.Exceptions;
    using System;

    public class CheckingAccount : BankAccount
    {
        public decimal OverdraftLimit { get; }

        private DateTime lastFeeDate = DateTime.MinValue;
        private bool feeAppliedToday;

        public CheckingAccount(string owner, string accountNumber, decimal overdraftLimit)
            : base(owner, accountNumber)
        {
            if (overdraftLimit < 0)
                throw new ArgumentOutOfRangeException(nameof(overdraftLimit));

            OverdraftLimit = overdraftLimit;
        }

        internal override void Withdraw(decimal amount)
        {
            ValidateAmount(amount);

            if (GetBalance() - amount < -OverdraftLimit)
            {
                throw new OverdraftLimitExceededException();
            }

            base.Withdraw(amount);
        }

        internal void ApplyOverdraftFee(decimal fee)
        {
            if (DateTime.UtcNow.Date != lastFeeDate)
            {
                feeAppliedToday = false;
                lastFeeDate = DateTime.UtcNow.Date;
            }

            if (GetBalance() < 0 && !feeAppliedToday)
            {
                ApplyFee(fee);
                feeAppliedToday = true;
            }
        }
    }
}
