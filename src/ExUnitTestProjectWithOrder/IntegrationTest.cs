    using Xunit;

[assembly:TestCaseOrderer("ExUnitTestProjectWithOrder.Extensions.DependencyTestCaseOrderer", "ExUnitTestProjectWithOrder")]
[assembly:CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

namespace ExUnitTestProjectWithOrder {
    using System;
    using Bank;
    using Extension;
    using Xunit;


    [CollectionDefinition(Name)]
    [TestCaseOrderer("ExUnitTestProjectWithOrder.Extensions.DependencyTestCaseOrderer", "ExUnitTestProjectWithOrder")]
    public class OrderedCollection : ICollectionFixture<OrderedFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.

        public const string Name = "Dependency Tests";
    }

    public sealed class OrderedFixture {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public OrderedFixture() {
        }
    }

    [Collection(OrderedCollection.Name)]
    [TestCaseOrderer("ExUnitTestProjectWithOrder.Extensions.DependencyTestCaseOrderer", "ExUnitTestProjectWithOrder")]
    public sealed class IntegrationTest {
        private readonly AccountRepository _accountRepository = new AccountRepository();

        [Fact]
        public void NewAccount_AccountRepository_CanSaveAccount() {
            // Given
            Account account = new Account("Foo", 10);

            // When
            this._accountRepository.Add(account);
            
            // Then
            Account persisted = this._accountRepository.Get("Foo");
            Assert.Equal(account, persisted);
        }

        [Fact]
        [TestDependency(typeof(IntegrationTest), nameof(NewAccount_AccountRepository_CanSaveAccount))]
        public void ExistingAccount_AccountRepository_CanRetrieveSavedAccount() {
            // Given 
            const string accountName = "Foo";

            // When
            Account account = this._accountRepository.Get(accountName);

            // Then
            Assert.Equal(account.Name, accountName);
        }

        [Fact]
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

        [Fact]
        [TestDependency(typeof(IntegrationTest), nameof(ExistingAccount_AccountRepository_CanDeleteSavedAccount))]
        public void NonExistingAccount_AccountRepository_GetThrows() {
            // Given 
            const string accountName = "Foo";

            // When / Then
            Assert.Throws<InvalidOperationException>(() => this._accountRepository.Get(accountName));
        }
    }
}