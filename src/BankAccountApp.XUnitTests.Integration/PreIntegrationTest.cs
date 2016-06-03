namespace BankAccountApp.XUnitTests.Integration {
    using Xunit;
    using XUnit.Extensions.TestOrdering;

    [Collection(nameof(PreIntegrationTest))]
    public sealed class PreIntegrationTest {
        private static bool _HasRun;

        [Fact]
        public void PreIntegrationTest_FirstStep() {
            _HasRun = true;

            Assert.True(true);
        }

        [Fact]
        [TestDependency(nameof(PreIntegrationTest_FirstStep))]
        public void PreIntegrationTest_SecondStep() {
            Assert.True(_HasRun);
        }
    }
}