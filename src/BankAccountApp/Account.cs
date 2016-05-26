namespace Bank {
    using System;
    using System.Diagnostics;
    using System.Globalization;

    public class Account {
        public string Name { get; set; }

        public decimal Balance { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Account(decimal balance=0) {
            this.Balance = balance;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Account(string name, decimal balance) {
            this.Balance = balance;
            this.Name = name;
        }

        public void Deposit(decimal amount) {
            this.Balance += amount;
        }

        public void Withdraw(decimal amount) {
            this.Balance -= amount; 
        }

        public void TransferFunds(Account destination, decimal amount) {
            Account.DoTransfer(this, destination, amount);
        }

        private static void DoTransfer(Account source, Account destination, decimal amount) {
            Debug.Assert(source != null);

            // checks to test
            if (destination == null) {
                throw new ArgumentNullException("destination");
            }

            if (amount <= 0) {
                throw new ArgumentOutOfRangeException("amount", "Amount must be bigger than zero");
            }

            // add
            checked {
                source.Withdraw(amount);

                try {
                    destination.Deposit(amount);
                } catch (OverflowException) {
                    source.Deposit(amount);
                    throw;
                }
            }
        }

        public override string ToString() {
            IFormattable fmt = $"{this.Name}: {this.Balance:C}";

            return fmt.ToString(null, CultureInfo.InvariantCulture);
        }
    }
}