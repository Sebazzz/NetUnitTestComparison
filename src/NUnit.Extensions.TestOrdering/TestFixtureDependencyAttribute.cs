namespace NUnit.Extensions.TestOrdering {
    using System;
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <summary>
    /// Needs to be applied to fixture which don't have a dependency, but other fixtures have a dependency on
    /// </summary>
    public class TestFixtureWithoutDependency : NUnitAttribute, IApplyToTest {
        public void ApplyToTest(Test test) {
            if (!test.Properties.ContainsKey(PropertyNames.Order)) {
                // NUnit sorts tests without an order set to be executed last and we want those tests to execute first.
                test.Properties.Set(PropertyNames.Order, 0);
            }
        }
    }

    /// <summary>
    ///     Defines the order that the test will run in
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TestFixtureDependencyAttribute : NUnitAttribute, IApplyToTest {
        public TestFixtureDependencyAttribute(Type fixtureDependency) {
            this.FixtureDependency = fixtureDependency;
        }

        public Type FixtureDependency { get; set; }

        /// <summary>
        ///     Modifies a test as defined for the specific attribute.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test) {
            if (!test.Properties.ContainsKey(PropertyNames.Order)) {
                test.Properties.Set(PropertyNames.Order, TestFixtureDependencyChainer.Instance.GetOrder(test));
            }
        }
    }
}