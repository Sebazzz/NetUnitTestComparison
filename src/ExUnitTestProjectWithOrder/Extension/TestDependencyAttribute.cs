namespace ExUnitTestProjectWithOrder.Extension {
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TestDependencyAttribute : Attribute {
        public TestDependencyAttribute(Type typeDependency) {
            this.TypeDependency = typeDependency;
        }

        public TestDependencyAttribute(Type typeDependency, string methodDependency) {
            this.MethodDependency = methodDependency;
            this.TypeDependency = typeDependency;
        }

        public Type TypeDependency { get; set; }

        public string MethodDependency { get; set; }
    }
}