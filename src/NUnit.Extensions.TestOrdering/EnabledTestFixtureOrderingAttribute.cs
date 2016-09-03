// ******************************************************************************
//  © 2016 Ernst & Young - www.ey.com | www.beco.nl
// 
//  Author          : Ernst & Young - Cleantech and Sustainability
//  File:           : OrderedTestAssemblyAttribute.cs
//  Project         : BankAccountApp.NUnitTests.Integration
// ******************************************************************************

namespace NUnit.Extensions.TestOrdering {
    using System;

    using Framework.Interfaces;
    using Framework.Internal;

    /// <summary>
    /// To be applied to assemblies which require tests to be ordered
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class EnabledTestFixtureOrderingAttribute : Attribute, IApplyToTest {
        public void ApplyToTest(Test test) {
            if (test == null) throw new ArgumentNullException(nameof(test));

            TestAssembly testAssembly = test as TestAssembly;

            if (testAssembly == null) {
                throw new ArgumentException($"Excepted argument \"{nameof(test)}\" to be of type {typeof(TestAssembly)} but was type {test.GetType()} instead", nameof(testAssembly));
            }

            OrderTestAssemblyTests(testAssembly);
        }

        private static void OrderTestAssemblyTests(TestAssembly testAssembly) {
            OrderedTreeBuilder treeBuilder = new OrderedTreeBuilder(testAssembly);
            treeBuilder.Add(testAssembly);
            treeBuilder.Complete();
        }
    }
}
