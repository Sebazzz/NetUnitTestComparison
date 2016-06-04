namespace BankAccountApp.NUnitTests.Unit {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public sealed class SetUpTearDown {
        [OneTimeSetUp]
        public void OneTimeSetUp() => Console.WriteLine(nameof(OneTimeSetUp));

        [OneTimeTearDown]
        public void OneTimeTearDown() => Console.WriteLine(nameof(OneTimeTearDown));

        [SetUp]
        public void SetUp() => Console.WriteLine(nameof(SetUp));

        [TearDown]
        public void TearDown() => Console.WriteLine(nameof(TearDown));

        [Test]
        public void OneTest() => Console.WriteLine(nameof(OneTest));

        [TestCase("A")]
        [TestCase("B")]
        public void TestCase(string str) => Console.WriteLine("{0}(\"{1}\")", nameof(TestCase), str);
    }
}