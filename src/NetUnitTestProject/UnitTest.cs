// ReSharper disable InconsistentNaming
// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable RedundantArgumentDefaultValue
namespace NetUnitTestProject {
    using System;
    using Bank;
    using NUnit.Framework;

    [TestFixture]
    public class UnitTest {
        [Test]
        public void NUnit_AccountTransfer_WhenGivenNullAccount_ThrowsArgumentNullException() {
            // given
            Account account = new Account();
            Account target = null;

            // then ( when )
            Assert.Throws<ArgumentNullException>(() => account.TransferFunds(target, 1));
        }

        [Test]
        public void NUnit_AccountTransfer_WhenGivenLessOrEqualToZeroAmount_ThrowsArgumentOutOfRangeException() {
            // given
            Account account = new Account();
            Account target = new Account();
            const decimal amount = 0;

            // then ( when )
            Assert.Throws<ArgumentOutOfRangeException>(() => account.TransferFunds(target, amount));
        }
        
        [Test]
        public void NUnit_AccountTransfer_WhenAmountOverflowsAccount_ThrowsOverflowException() {
            // given
            Account account = new Account(10);
            Account target = new Account(Decimal.MaxValue - 1);
            const decimal amount = 5;

            // then ( when )
            Assert.Throws<OverflowException>(() => account.TransferFunds(target, amount));
        }

        [Test]
        public void NUnit_AccountTransfer_WhenAmount_TransfersCorrectly() {
            // given
            Account account = new Account(25);
            Account target = new Account(0);
            const decimal amount = 10;

            // when
            account.TransferFunds(target, amount);

            // then
            // ... this:
            Assert.That(account.Balance, Is.EqualTo(25 - 10));
            Assert.That(target.Balance, Is.EqualTo(10));

            // ... is the same as:
            Assert.AreEqual(25 - 10, account.Balance);
            Assert.AreEqual(10, target.Balance);
        }

        [Test]
        public void NUnit_AccountTransfer_WhenOverflow_TransferCorrectlyCancelled() {
            // given
            Account account = new Account(10);
            Account target = new Account(Decimal.MaxValue - 1);
            const decimal amount = 5;

            // when
            try {
                account.TransferFunds(target, amount);
            } catch (OverflowException) {
                // ignore for this test
            }

            // then
            Assert.AreEqual(10, account.Balance);
            Assert.AreEqual(Decimal.MaxValue - 1, target.Balance);
        }
    }
}
