namespace XUnit.Extensions.TestOrdering {
    using System;

    /// <summary>
    /// Specifies on which test the current test is dependend for execution within the same fixture
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TestDependencyAttribute : Attribute {
        public TestDependencyAttribute(string methodDependency) {
            this.MethodDependency = methodDependency;
        }

        public string MethodDependency { get; }
    }
}