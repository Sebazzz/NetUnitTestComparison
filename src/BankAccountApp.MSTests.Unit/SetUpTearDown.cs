namespace BankAccountApp.MSTests.Unit {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public sealed class SetUpTearDown {

        [ClassInitialize]
        public static void ClassInitialize(TestContext ctx) => Console.WriteLine(nameof(ClassInitialize));

        [ClassCleanup]
        public static void ClassCleanup() => Console.WriteLine(nameof(ClassCleanup));

        [TestInitialize]
        public void TestInitialize() => Console.WriteLine(nameof(TestInitialize));

        [TestCleanup]
        public void TestCleanup() => Console.WriteLine(nameof(TestCleanup));

        [TestMethod]
        public void TestMethod() => Console.WriteLine(nameof(TestMethod));

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext ctx) => Console.WriteLine(nameof(AssemblyInitialize));

        [AssemblyCleanup]
        public static void AssemblyCleanup() => Console.WriteLine(nameof(AssemblyCleanup));
    }
}