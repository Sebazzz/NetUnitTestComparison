﻿// ReSharper disable InconsistentNaming
// ReSharper disable ExpressionIsAlwaysNull
namespace BankAccountApp.NUnitTests.Integration {
    using System;
    using Bank;
    using NUnit.Extensions.TestOrdering;
    using NUnit.Framework;

    [TestFixture]
    [TestFixtureDependency(typeof(PreIntegrationTest))]
    public sealed class IntegrationTest {
        private readonly AccountRepository _accountRepository = new AccountRepository();

        [Test]
        [TestMethodDependency(nameof(ExistingAccount_AccountRepository_CanRetrieveSavedAccount))]
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
        [TestMethodDependency(nameof(ExistingAccount_AccountRepository_CanDeleteSavedAccount))]
        public void NonExistingAccount_AccountRepository_GetThrows() {
            // Given 
            const string accountName = "Foo";

            // When / Then
            Assert.Throws<InvalidOperationException>(() => this._accountRepository.Get(accountName));
        }

        [Test]
        [TestMethodWithoutDependency]
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
        [TestMethodDependency(nameof(NewAccount_AccountRepository_CanSaveAccount))]
        public void ExistingAccount_AccountRepository_CanRetrieveSavedAccount() {
            // Given 
            const string accountName = "Foo";

            // When
            Account account = this._accountRepository.Get(accountName);

            // Then
            Assert.AreEqual(account.Name, accountName);
        }
    }
}
