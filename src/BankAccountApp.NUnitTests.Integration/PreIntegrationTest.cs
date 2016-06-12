namespace BankAccountApp.NUnitTests.Integration {
    using NUnit.Extensions.TestOrdering;
    using NUnit.Framework;

    [TestFixture]
    public sealed class PreIntegrationTest {
        private static bool _HasRun;

        [Test]
        [TestMethodWithoutDependency]
        public void PreIntegrationTest_FirstStep() {
            _HasRun = true;

            Assert.True(true);
        }

        [Test]
        [TestMethodDependency(nameof(PreIntegrationTest_FirstStep))]
        public void PreIntegrationTest_SecondStep() {
            Assert.True(_HasRun);
        }
    }
}