namespace BankAccountApp.NUnitTests.Integration.ChildNamespace {
    using NUnit.Framework;

    [TestFixture]
    public class ChildTestFixtureWithoutOrder {
        [Test]
        public void Test1() {}

        [Test]
        public void Test2() {}
    }
}