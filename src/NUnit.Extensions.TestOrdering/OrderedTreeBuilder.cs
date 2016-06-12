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

        public TestSuite Root { get; }

        public OrderedTreeBuilder(Assembly assembly) {
            this._orderedTests = new OrderedTestSuite("Ordered tests");
            this._unorderedTests = new TestSuite("Other tests");

            this.Root = new TestSuite(assembly.FullName);
            this.Root.Add(this._unorderedTests);
            this.Root.Add(this._orderedTests);

            this._fixtureOrderingCache = new FixtureOrderingCache(assembly);
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
            foreach (Test childTest in suite.Tests.Cast<Test>()) {
                this._unorderedTests.Add(childTest);
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

                    if (!parentWrapper.TestSuite.Tests.Contains(testCollectionWrapper.TestSuite))
                        parentWrapper.TestSuite.Add(testCollectionWrapper.TestSuite);
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
                    int childTypeIndex = testSuite.Tests.IndexOf(childTypeTestSuite);

                    // Swap
                    Test testAtTargetIndex = (Test) testSuite.Tests[i];
                    testSuite.Tests[i] = childTypeTestSuite;
                    testSuite.Tests[childTypeIndex] = testAtTargetIndex;
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

            foreach (Test childTest in test.Tests.Cast<Test>().ToList()) {
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