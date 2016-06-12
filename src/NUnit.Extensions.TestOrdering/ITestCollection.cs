namespace NUnit.Extensions.TestOrdering {
    using System;
    using System.Collections.Generic;

    public interface ITestCollection {
        IEnumerable<Type> GetTestFixtures();

        bool ContinueOnFailure { get; }
    }
}