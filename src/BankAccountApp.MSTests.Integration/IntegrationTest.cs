namespace BankAccountApp.MSTests.Integration {
    using System;
    using Bank;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class IntegrationTest {
        private readonly AccountRepository _accountRepository = new AccountRepository();

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExistingAccount_AccountRepository_CanDeleteSavedAccount() {
            // Given 
            const string accountName = "Foo";

            // When
            var result = this._accountRepository.Delete(accountName);

            // Then
            Assert.IsTrue(result);
            this._accountRepository.Get(accountName); // Expect exception
        }

        [TestMethod]
        public void ExistingAccount_AccountRepository_CanRetrieveSavedAccount() {
            // Given 
            const string accountName = "Foo";

            // When
            var account = this._accountRepository.Get(accountName);

            // Then
            Assert.AreEqual(account.Name, accountName);
        }

        [TestMethod]
        public void NewAccount_AccountRepository_CanSaveAccount() {
            // Given
            var account = new Account("Foo", 10);

            // When
            this._accountRepository.Add(account);

            // Then
            var persisted = this._accountRepository.Get("Foo");
            Assert.AreEqual(account, persisted);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NonExistingAccount_AccountRepository_GetThrows() {
            // Given 
            const string accountName = "Foo";

            // When / Then
            this._accountRepository.Get(accountName);
        }
    }
}