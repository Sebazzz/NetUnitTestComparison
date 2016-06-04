namespace BankAccountApp.MSTests.Unit {
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class Assertions {
        [TestMethod]
        public void TestA() => Assert.AreEqual(10, 20);

        [TestMethod]
        public void TestB() => Assert.AreEqual("ABC", "ABC!");

        [TestMethod]
        public void TestC() => CollectionAssert.AreEqual(new[]{"A", "B"}, new []{"A", "C"});
    }
}