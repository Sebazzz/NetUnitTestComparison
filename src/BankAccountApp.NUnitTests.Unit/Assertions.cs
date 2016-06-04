namespace BankAccountApp.NUnitTests.Unit {
    using NUnit.Framework;

    [TestFixture]
    public sealed class Assertions {
        [Test]
        public void TestA() => Assert.AreEqual(10, 20);

        [Test]
        public void TestB() => Assert.AreEqual("ABC", "ABC!");

        [Test]
        public void TestC() => CollectionAssert.AreEqual(new[] {"A", "B"}, new[] {"A", "C"});
    }
}