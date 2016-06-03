namespace XUnit.Extensions.TestOrdering {
    using System;

    /// <summary>
    /// Specifies on which test collection this test collection has a dependency
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TestCollectionDependencyAttribute : Attribute {
        public Type Dependency { get; }

        public TestCollectionDependencyAttribute(Type dependency) {
            this.Dependency = dependency;
        }
    }
}