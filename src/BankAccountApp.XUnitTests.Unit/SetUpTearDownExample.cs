namespace BankAccountApp.XUnitTests.Unit {
    using System;
    using Xunit;

    [CollectionDefinition("Database collection")]
    public class MyCollection : ICollectionFixture<SharedBetweenTestClassesInCollection>,
        IClassFixture<SharedBetweenTestsInSameClass> {}

    public class SharedBetweenTestClassesInCollection : IDisposable {
        public SharedBetweenTestClassesInCollection() {
            Console.WriteLine("SetUp: One per 'MyCollection'");
        }

        public void Dispose() => Console.WriteLine("TearDown: One per 'MyCollection'");
    }

    public class SharedBetweenTestsInSameClass : IDisposable {
        public SharedBetweenTestsInSameClass() {
            Console.WriteLine("SetUp: One per test class");
        }

        public void Dispose() => Console.WriteLine("TearDown: One per test class");
    }

    [Collection("MyCollection")]
    public class TestClassA : IDisposable {
        public TestClassA(SharedBetweenTestsInSameClass _, SharedBetweenTestClassesInCollection __) {
            Console.WriteLine("SetUp: One per test method");
        }

        public void Dispose() => Console.WriteLine("TearDown: One per test method");

        [Fact]
        public void TestA1() => Console.WriteLine("Hello Test A 1");

        [Fact]
        public void TestA2() => Console.WriteLine("Hello Test A 2");
    }

    [Collection("MyCollection")]
    public class TestClassB : IDisposable {
        public TestClassB(SharedBetweenTestsInSameClass _, SharedBetweenTestClassesInCollection __) {
            Console.WriteLine("SetUp: One per test method");
        }

        public void Dispose() => Console.WriteLine("TearDown: One per test method");

        [Fact]
        public void TestB() => Console.WriteLine("Hello TestB");
    }
}