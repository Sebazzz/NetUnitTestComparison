namespace BankAccountApp.NUnitTests.Integration {
    using NUnit.Extensions.TestOrdering;
    using NUnit.Framework;

    [TestFixture]
    public sealed class PostIntegrationTest {
        private static bool _HasRun;

        [Test]
        [TestMethodWithoutDependency]
        public void PostIntegrationTest_FirstStep() {
            _HasRun = true;

            Assert.True(true);
        }

        [Test]
        [TestMethodDependency(nameof(PostIntegrationTest_FirstStep))]
        public void PostIntegrationTest_SecondStep() {
            Assert.True(_HasRun);
        }
    }
}