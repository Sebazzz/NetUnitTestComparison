// ReSharper disable InconsistentNaming
// ReSharper disable ExpressionIsAlwaysNull

namespace MicrosoftTestProject {
    using System;
    using Bank;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTest {
        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void MSTest_AccountTransfer_WhenGivenNullAccount_ThrowsArgumentNullException() {
            // given
            var account = new Account();
            Account target = null;

            // when
            account.TransferFunds(target, 1);

            // then (should not reach this)
            //Assert.Fail("Excepted an ArgumentNullException to be thrown");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void MSTest_AccountTransfer_WhenGivenLessOrEqualToZeroAmount_ThrowsArgumentOutOfRangeException() {
            // given
            var account = new Account();
            var target = new Account();
            const decimal amount = 0;

            // then ( when )
            account.TransferFunds(target, amount);

            // then (should not reach this)
            //Assert.Fail("Excepted an ArgumentNullException to be thrown");
        }

        [TestMethod]
        [ExpectedException(typeof (OverflowException))]
        public void MSTest_AccountTransfer_WhenAmountOverflowsAccount_ThrowsOverflowException() {
            // given
            var account = new Account(10);
            var target = new Account(Decimal.MaxValue - 1);
            const decimal amount = 5;

            // then ( when )
            account.TransferFunds(target, amount);

            // then (should not reach this)
            //Assert.Fail("Excepted an ArgumentNullException to be thrown");
        }

        [TestMethod]
        public void MSTest_AccountTransfer_WhenAmount_TransfersCorrectly() {
            // given
            var account = new Account(25);
            var target = new Account(0);
            const decimal amount = 10;

            // when
            account.TransferFunds(target, amount);

            // then
            Assert.AreEqual(25 - 10, account.Balance);
            Assert.AreEqual(10, target.Balance);
        }

        [TestMethod]
        public void MSTest_AccountTransfer_WhenOverflow_TransferCorrectlyCancelled() {
            // given
            var account = new Account(10);
            var target = new Account(Decimal.MaxValue - 1);
            const decimal amount = 5;

            // when
            try {
                account.TransferFunds(target, amount);
            }
            catch (OverflowException) {
                // ignore for this test
            }

            // then
            Assert.AreEqual(10, account.Balance);
            Assert.AreEqual(Decimal.MaxValue - 1, target.Balance);
        }
    }
}