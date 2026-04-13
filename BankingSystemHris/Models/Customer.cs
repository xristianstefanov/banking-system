namespace BankingSystemHris.Models
{
    using System.Collections.Generic;
    public class Customer
    {
        public string Id { get; }
        public string Name { get; }

        private readonly List<BankAccount> accounts = new();

        public IReadOnlyList<BankAccount> Accounts => accounts.AsReadOnly();

        public Customer(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public void AddAccount(BankAccount account)
        {
            accounts.Add(account);
        }
    }
}
