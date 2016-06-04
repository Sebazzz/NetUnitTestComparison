namespace BankAccountApp.XUnitTests.Unit {
    using Xunit;

    public sealed class Assertions {
        [Fact]
        public void TestA() => Assert.Equal(10, 20);

        [Fact]
        public void TestB() => Assert.Equal("ABC", "ABC!");

        [Fact]
        public void TestC() => Assert.Equal(new[] {"A", "B"}, new[] {"A", "C"});
    }
}