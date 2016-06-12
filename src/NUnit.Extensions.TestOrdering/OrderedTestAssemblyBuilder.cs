namespace NUnit.Extensions.TestOrdering {
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Framework.Api;
    using Framework.Interfaces;
    using Framework.Internal;

    public sealed class OrderedTestAssemblyBuilder : ITestAssemblyBuilder {
        private readonly DefaultTestAssemblyBuilder _defaultTestAssemblyBuilder;

        public OrderedTestAssemblyBuilder() {
            this._defaultTestAssemblyBuilder = new DefaultTestAssemblyBuilder();
        }

        /// <summary>Build a suite of tests from a provided assembly</summary>
        /// <param name="assembly">The assembly from which tests are to be built</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>A TestSuite containing the tests found in the assembly</returns>
        public ITest Build(Assembly assembly, IDictionary<string,object> options) {
            TestAssembly testAssembly = (TestAssembly) this._defaultTestAssemblyBuilder.Build(assembly, options);

            if (testAssembly.RunState == RunState.NotRunnable) {
                return testAssembly;
            }

            return CreateOrderedTestHierarchy(testAssembly, assembly);
        }

        /// <summary>
        /// Build a suite of tests given the filename of an assembly
        /// </summary>
        /// <param name="assemblyName">The filename of the assembly from which tests are to be built</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>A TestSuite containing the tests found in the assembly</returns>
        public ITest Build(string assemblyName, IDictionary<string,object> options) {
            TestAssembly testAssembly = (TestAssembly) this._defaultTestAssemblyBuilder.Build(assemblyName, options);
            
            if (testAssembly.RunState == RunState.NotRunnable) {
                return testAssembly;
            }

            return CreateOrderedTestHierarchy(testAssembly, Assembly.Load(assemblyName));
        }

        private static TestSuite CreateOrderedTestHierarchy(TestAssembly testAssembly, Assembly assembly) {
            //Debugger.Launch();

            OrderedTreeBuilder treeBuilder = new OrderedTreeBuilder(assembly);
            treeBuilder.Add(testAssembly);

            return treeBuilder.Root;
        }
    }
}