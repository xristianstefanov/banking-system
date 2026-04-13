namespace BankingSystemHris.Models
{
    using BankingSystemHris.Exceptions;
    public class SavingsAccount : BankAccount
    {
        public decimal InterestRate { get; }
        public decimal MaxBalance { get; }

        public SavingsAccount(string owner, string accNum, decimal rate, decimal maxBalance)
            : base(owner, accNum)
        {
            InterestRate = rate;
            MaxBalance = maxBalance;
        }

        internal void ApplyInterest(CompoundingMode mode)
        {
            decimal divisor = mode == CompoundingMode.Monthly ? 12 : 1;
            decimal interest = GetBalance() * (InterestRate / divisor);

            if (GetBalance() + interest > MaxBalance)
            {
                throw new InvalidOperationException("Maximum balance exceeded.");
            }

            Deposit(interest);
            AddTransaction(TransactionType.Interest, interest, to: AccountNumber);
        }
    }
}
