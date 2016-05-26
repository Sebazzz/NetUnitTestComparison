namespace Bank {
    using System;
    using System.Collections.Generic;

    public sealed class AccountRepository {
        private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;
        private static readonly List<Account> AccountList = new List<Account>();

        public void Add(Account account) {
            AccountList.Add(account);
        }

        public bool Delete(string accountName) {
            return AccountList.RemoveAll(AccountMatcher(accountName)) > 0;
        }

        private static Predicate<Account> AccountMatcher(string accountName) {
            return account => StringComparer.Equals(account.Name, accountName);
        }

        public Account Get(string accountName) {
            Account account = AccountList.Find(AccountMatcher(accountName));

            if (account == null) {
                throw new InvalidOperationException($"Account with name '{accountName} not found. Accounts available: '" + 
                    String.Join(";; ", AccountList));
            }

            return account;
        }
    }
}