// ReSharper disable InconsistentNaming
// ReSharper disable ExpressionIsAlwaysNull
namespace NetUnitTestProjectWithOrder {
    using System;
    using Bank;
    using Extensions;
    using NUnit.Framework;

    [TestFixture]
    public sealed class IntegrationTest {
        private readonly AccountRepository _accountRepository = new AccountRepository();

        [Test]
        public void NewAccount_AccountRepository_CanSaveAccount() {
            // Given
            Account account = new Account("Foo", 10);

            // When
            this._accountRepository.Add(account);
            
            // Then
            Account persisted = this._accountRepository.Get("Foo");
            Assert.AreEqual(account, persisted);
        }

        [Test]
        [TestDependency(typeof(IntegrationTest), nameof(NewAccount_AccountRepository_CanSaveAccount))]
        public void ExistingAccount_AccountRepository_CanRetrieveSavedAccount() {
            // Given 
            const string accountName = "Foo";

            // When
            Account account = this._accountRepository.Get(accountName);

            // Then
            Assert.AreEqual(account.Name, accountName);
        }

        [Test]
        [TestDependency(typeof(IntegrationTest), nameof(ExistingAccount_AccountRepository_CanRetrieveSavedAccount))]
        public void ExistingAccount_AccountRepository_CanDeleteSavedAccount() {
            // Given 
            const string accountName = "Foo";

            // When
            bool result = this._accountRepository.Delete(accountName);

            // Then
            Assert.True(result);
            Assert.Throws<InvalidOperationException>(() => this._accountRepository.Get(accountName));
        }

        [Test]
        [TestDependency(typeof(IntegrationTest), nameof(ExistingAccount_AccountRepository_CanDeleteSavedAccount))]
        public void NonExistingAccount_AccountRepository_GetThrows() {
            // Given 
            const string accountName = "Foo";

            // When / Then
            Assert.Throws<InvalidOperationException>(() => this._accountRepository.Get(accountName));
        }
    }
}
