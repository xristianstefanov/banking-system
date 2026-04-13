namespace BankingSystemHris.Models
{
    public class Transaction
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public TransactionType Type { get; }
        public decimal Amount { get; }
        public string FromAccount { get; }
        public string ToAccount { get; }
        public decimal ResultingBalance { get; }

        public Transaction(TransactionType type, decimal amount, decimal resultingBalance,
            string from = null, string to = null)
        {
            Type = type;
            Amount = amount;
            ResultingBalance = resultingBalance;
            FromAccount = from;
            ToAccount = to;
        }
    }
}
