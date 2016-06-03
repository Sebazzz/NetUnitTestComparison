namespace BankAccountApp.XUnitTests.Integration {
    using Xunit;
    using XUnit.Extensions.TestOrdering;

    [Collection(nameof(PostIntegrationTest))]
    [TestCollectionDependency(typeof(IntegrationTest))]
    public sealed class PostIntegrationTest {
        private static bool _HasRun;

        [Fact]
        public void PostIntegrationTest_FirstStep() {
            _HasRun = true;

            Assert.True(true);
        }

        [Fact]
        [TestDependency(nameof(PostIntegrationTest_FirstStep))]
        public void PostIntegrationTest_SecondStep() {
            Assert.True(_HasRun);
        }
    }
}