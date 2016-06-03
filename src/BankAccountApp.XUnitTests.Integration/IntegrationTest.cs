namespace BankAccountApp.XUnitTests.Integration {
    using System;
    using Bank;
    using Xunit;
    using XUnit.Extensions.TestOrdering;

    [Collection(nameof(IntegrationTest))]
    [TestCollectionDependency(typeof(PreIntegrationTest))]
    public sealed class IntegrationTest {
        private readonly AccountRepository _accountRepository = new AccountRepository();

        [Fact]
        [TestDependency(nameof(ExistingAccount_AccountRepository_CanRetrieveSavedAccount))]
        public void ExistingAccount_AccountRepository_CanDeleteSavedAccount() {
            // Given 
            const string accountName = "Foo";

            // When
            var result = this._accountRepository.Delete(accountName);

            // Then
            Assert.True(result);
            Assert.Throws<InvalidOperationException>(() => this._accountRepository.Get(accountName));
        }

        [Fact]
        [TestDependency(nameof(NewAccount_AccountRepository_CanSaveAccount))]
        public void ExistingAccount_AccountRepository_CanRetrieveSavedAccount() {
            // Given 
            const string accountName = "Foo";

            // When
            var account = this._accountRepository.Get(accountName);

            // Then
            Assert.Equal(account.Name, accountName);
        }

        [Fact]
        public void NewAccount_AccountRepository_CanSaveAccount() {
            // Given
            var account = new Account("Foo", 10);

            // When
            this._accountRepository.Add(account);

            // Then
            var persisted = this._accountRepository.Get("Foo");
            Assert.Equal(account, persisted);
        }

        [Fact]
        [TestDependency(nameof(ExistingAccount_AccountRepository_CanDeleteSavedAccount))]
        public void NonExistingAccount_AccountRepository_GetThrows() {
            // Given 
            const string accountName = "Foo";

            // When / Then
            Assert.Throws<InvalidOperationException>(() => this._accountRepository.Get(accountName));
        }
    }
}