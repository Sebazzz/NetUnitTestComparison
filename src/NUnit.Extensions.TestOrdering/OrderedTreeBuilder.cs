namespace NUnit.Extensions.TestOrdering {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Framework.Interfaces;
    using Framework.Internal;

    public sealed class OrderedTreeBuilder {
        private readonly TestSuite _orderedTests;
        private readonly TestSuite _unorderedTests;

        private readonly FixtureOrderingCache _fixtureOrderingCache;

        private readonly Dictionary<Type, TestCollectionWrapper> _testSuitesByType;
        private readonly Dictionary<TestCollectionInfo, TestCollectionWrapper> _testSuitesByTestCollection;

        public TestAssembly Root { get; }


        public void Complete() {
            this.Root.Add(this._unorderedTests);
            this.Root.Add(this._orderedTests);

            this._unorderedTests.Properties.Set(PropertyNames.Order, 0);
            this._orderedTests.Properties.Set(PropertyNames.Order, 1);
        }

        public OrderedTreeBuilder(TestAssembly testAssembly) {
            this._unorderedTests = new TestSuite("Unordered");
            this._orderedTests = new OrderedTestSuite("Ordered");

            this.Root = testAssembly;
            
            this._fixtureOrderingCache = new FixtureOrderingCache(testAssembly.Assembly);
            this._testSuitesByType = new Dictionary<Type, TestCollectionWrapper>();
            this._testSuitesByTestCollection = new Dictionary<TestCollectionInfo, TestCollectionWrapper>();

            this.BuildTestSuites();
        }

        private void BuildTestSuites() {
            foreach (TestCollectionInfo testCollection in this._fixtureOrderingCache.TestCollections) {
                foreach (Type fixtureType in testCollection.ChildTypes) {
                    this._testSuitesByType[fixtureType] = new TestCollectionWrapper(testCollection);
                    this._testSuitesByTestCollection[testCollection] = new TestCollectionWrapper(testCollection) {
                        TestSuite = new TestCollectionTestSuite(testCollection.TypeName)
                    };
                }
            }
        }

        public void Add(TestSuite suite) {
            this.AddFromHierarchy(null, suite);

            this.InitializeHierarchy();

            // The given test suite is not all that interesting, but the children are
            for (int i = suite.Tests.Count - 1; i >= 0; i--) {
                ITest childTest = suite.Tests[i];

                this._unorderedTests.Add((Test) childTest);

                suite.Tests.RemoveAt(i);
            }
        }

        private void InitializeHierarchy() {
            foreach (TestCollectionWrapper testCollectionWrapper in this._testSuitesByTestCollection.Values) {
                TestCollectionInfo childCollection = testCollectionWrapper.TestCollection;
                TestCollectionInfo parent = testCollectionWrapper.TestCollection.Parent;

                if (parent != null) {
                    TestCollectionWrapper parentWrapper;
                    if (!this._testSuitesByTestCollection.TryGetValue(parent, out parentWrapper)) {
                        throw new InvalidOperationException($"Unable to find parent of {childCollection.TypeName} (expected: {childCollection.Parent.TypeName}");
                    }

                    if (!parentWrapper.TestSuite.Tests.Contains(testCollectionWrapper.TestSuite)) {
                        parentWrapper.TestSuite.Add(testCollectionWrapper.TestSuite);
                    }
                }
                else if (!this._orderedTests.Tests.Contains(testCollectionWrapper.TestSuite)) {
                    this._orderedTests.Tests.Add(testCollectionWrapper.TestSuite);
                }
            }

            this.SortTestCollections();
        }

        private void SortTestCollections() {
            foreach (TestCollectionWrapper wrapper in this._testSuitesByTestCollection.Values) {
                TestCollectionTestSuite testSuite = wrapper.TestSuite;

                for (int i = 0; i < wrapper.TestCollection.ChildTypes.Length; i++) {
                    Type childType = wrapper.TestCollection.ChildTypes[i];

                    Test childTypeTestSuite = this._testSuitesByType[childType].TestSuite;
                    int childTypeTestSuiteIndex = testSuite.Tests.IndexOf(childTypeTestSuite);

                    // Swap
                    Test testAtTargetIndex = (Test) testSuite.Tests[i];
                    testSuite.Tests[i] = childTypeTestSuite;
                    testSuite.Tests[childTypeTestSuiteIndex] = testAtTargetIndex;

                    childTypeTestSuite.Properties.Set(PropertyNames.Order, i);
                }
            }
        }

        private void AddFromHierarchy(Test parent, Test test) {
            if (test.TypeInfo != null) {
                Type type = ((TypeWrapper) test.TypeInfo).Type;

                if (this.AddFixtureType(test, type)) {
                    parent?.Tests.Remove(test);
                }
                return;
            }

            for (int i = test.Tests.Count - 1; i >= 0; i--) {
                Test childTest = (Test) test.Tests[i];

                this.AddFromHierarchy(test, childTest);
            }
        }

        private bool AddFixtureType(Test test, Type type) {
            TestCollectionWrapper testCollectionWrapper;
            if (!this._testSuitesByType.TryGetValue(type, out testCollectionWrapper)) {
                return false;
            }

            TestCollectionTestSuite testSuite = GetTestSuite(test, testCollectionWrapper);

            TestCollectionWrapper parentWrapper;
            if (!this._testSuitesByTestCollection.TryGetValue(testSuite.TestCollection, out parentWrapper)) {
                throw new InvalidOperationException($"Unable to find parent of {type} (expected: {testSuite.TestCollection.TypeName}");
            }

            parentWrapper.TestSuite.Add(testSuite);

            return true;
        }

        private static TestCollectionTestSuite GetTestSuite(Test test, TestCollectionWrapper testCollectionWrapper) {
            TestCollectionTestSuite testSuite = testCollectionWrapper.TestSuite;

            if (testSuite == null) {
                testSuite = new TestCollectionTestSuite(test, testCollectionWrapper.TestCollection);
                testCollectionWrapper.TestSuite = testSuite;
            }

            return testSuite;
        }
    }

    public class OrderedTestSuite : TestSuite {
        public OrderedTestSuite(Test original) : base(original.TypeInfo) {
            this.MaintainTestOrder = true;
        }

        public OrderedTestSuite(string name) : base(name) {}
    }

    public class TestCollectionTestSuite : OrderedTestSuite {
        public TestCollectionInfo TestCollection { get; }

        public TestCollectionTestSuite(Test original, TestCollectionInfo testCollection) : base(original) {
            this.TestCollection = testCollection;
            this.MaintainTestOrder = true;

            foreach (ITest test in original.Tests) {
                this.Add((Test) test);
            }
        }

        public TestCollectionTestSuite(string name) : base(name) {}
    }

    public class TestCollectionWrapper {
        public TestCollectionInfo TestCollection { get; }

        public TestCollectionTestSuite TestSuite { get; set; }

        public TestCollectionWrapper(TestCollectionInfo testCollection) {
            this.TestCollection = testCollection;
        }
    }
}