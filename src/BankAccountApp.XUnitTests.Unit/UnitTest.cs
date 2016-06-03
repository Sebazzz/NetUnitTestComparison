// ReSharper disable InconsistentNaming
// ReSharper disable ExpressionIsAlwaysNull
namespace BankAccountApp.XUnitTests.Unit {
    using System;
    using Bank;
    using Xunit;

    public class UnitTest {
        [Fact]
        public void XUnit_AccountTransfer_WhenGivenNullAccount_ThrowsArgumentNullException() {
            // given
            Account account = new Account();
            Account target = null;

            // then ( when )
            Assert.Throws<ArgumentNullException>(() => account.TransferFunds(target, 1));
        }

        [Fact]
        public void XUnit_AccountTransfer_WhenGivenLessOrEqualToZeroAmount_ThrowsArgumentOutOfRangeException() {
            // given
            Account account = new Account();
            Account target = new Account();
            const decimal amount = 0;

            // then ( when )
            Assert.Throws<ArgumentOutOfRangeException>(() => account.TransferFunds(target, amount));
        }

        [Fact]
        public void XUnit_AccountTransfer_WhenAmountOverflowsAccount_ThrowsOverflowException() {
            // given
            Account account = new Account(10);
            Account target = new Account(Decimal.MaxValue - 1);
            const decimal amount = 5;

            // then ( when )
            Assert.Throws<OverflowException>(() => account.TransferFunds(target, amount));
        }

        [Fact]
        public void XUnit_AccountTransfer_WhenAmount_TransfersCorrectly() {
            // given
            Account account = new Account(25);
            Account target = new Account(0);
            const decimal amount = 10;

            // when
            account.TransferFunds(target, amount);

            // then
            Assert.Equal(25 - 10, account.Balance);
            Assert.Equal(10, target.Balance);
        }

        [Fact]
        public void XUnit_AccountTransfer_WhenOverflow_TransferCorrectlyCancelled() {
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
            Assert.Equal(10, account.Balance);
            Assert.Equal(Decimal.MaxValue - 1, target.Balance);
        }
    }
}
