namespace BankAccountApp.MSTests.Integration {
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class PreIntegrationTest {
        private static bool _HasRun;

        [TestMethod]
        public void PreIntegrationTest_FirstStep() {
            _HasRun = true;

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PreIntegrationTest_SecondStep() {
            Assert.IsTrue(_HasRun);
        }
    }
}