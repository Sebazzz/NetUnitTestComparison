namespace BankAccountApp.MSTests.Integration {
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class PostIntegrationTest {
        private static bool _HasRun;

        [TestMethod]
        public void PostIntegrationTest_FirstStep() {
            _HasRun = true;

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PostIntegrationTest_SecondStep() {
            Assert.IsTrue(_HasRun);
        }
    }
}