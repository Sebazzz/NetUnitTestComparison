namespace BankAccountApp.NUnitTests.Integration.Extensions {
    using System;
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <summary>
    ///     Defines the order that the test will run in
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestDependencyAttribute : NUnitAttribute, IApplyToTest {
        public TestDependencyAttribute(Type typeDependency) {
            this.TypeDependency = typeDependency;
        }

        public TestDependencyAttribute(Type typeDependency, string methodDependency) {
            this.MethodDependency = methodDependency;
            this.TypeDependency = typeDependency;
        }

        public Type TypeDependency { get; set; }

        public string MethodDependency { get; set; }

        /// <summary>
        ///     Modifies a test as defined for the specific attribute.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test) {
            if (!test.Properties.ContainsKey(PropertyNames.Order))
                test.Properties.Set(PropertyNames.Order, TestDependencyChainer.Instance.GetOrder(test));
        }
    }
}