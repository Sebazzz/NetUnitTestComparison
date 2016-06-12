namespace BankAccountApp.NUnitTests.Integration {
    using System;
    using System.Collections.Generic;
    using NUnit.Extensions.TestOrdering;
    public sealed class MyTestCollection : ITestCollection {
        public IEnumerable<Type> GetTestFixtures() {
            yield return typeof(PreIntegrationTest);
            yield return typeof(IntegrationTest);
            yield return typeof(PostIntegrationTest);
        }

        public bool ContinueOnFailure => false;
    }
}