namespace ExUnitTestProjectWithOrder.Extension {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public sealed class DependencyTestCaseOrderer : ITestCaseOrderer {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DependencyTestCaseOrderer() {
            throw new Exception();
        }

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase {
            var listOfTestCases = testCases.ToList();

            Debugger.Break();

            var testList = new List<TestCaseWrapper>(listOfTestCases.Select(x => new TestCaseWrapper(x)));
            testList.Sort(new TestCaseComparer());

            return testList.Select(x => x.TestCase).Cast<TTestCase>();
        }

        private sealed class TestCaseComparer : Comparer<TestCaseWrapper> {
            /// <summary>
            ///     When overridden in a derived class, performs a comparison of two objects of the same type and returns a value
            ///     indicating whether one object is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as
            ///     shown in the following table.Value Meaning Less than zero <paramref name="x" /> is less than <paramref name="y" />
            ///     .Zero <paramref name="x" /> equals <paramref name="y" />.Greater than zero <paramref name="x" /> is greater than
            ///     <paramref name="y" />.
            /// </returns>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <exception cref="T:System.ArgumentException">
            ///     Type <paramref name="T" /> does not implement either the
            ///     <see cref="T:System.IComparable`1" /> generic interface or the <see cref="T:System.IComparable" /> interface.
            /// </exception>
            public override int Compare(TestCaseWrapper x, TestCaseWrapper y) {
                if (x == null || y == null) {
                    throw new InvalidOperationException();
                }

                if (x.IsDependency(y)) {
                    return -1; // x is less than y
                }

                if (y.IsDependency(x)) {
                    return 1; // y is less than x
                }

                // Follow alphabetic ordering
                var strComparison = StringComparer.Ordinal.Compare(x.TestType, y.TestType);
                if (strComparison == 0) strComparison = StringComparer.Ordinal.Compare(x.TestMethod, y.TestMethod);

                return strComparison;
            }
        }

        private sealed class TestCaseWrapper {
            private readonly string _dependendOnMethod;
            private readonly string _dependendOnType;

            public TestCaseWrapper(ITestCase testCase) {
                this.TestCase = testCase;

                var attributeInfo =
                    testCase.TestMethod.Method.GetCustomAttributes(typeof(TestDependencyAttribute))
                        .OfType<ReflectionAttributeInfo>()
                        .SingleOrDefault();

                if (attributeInfo != null) {
                    var attribute = (TestDependencyAttribute) attributeInfo.Attribute;

                    this._dependendOnType = attribute.TypeDependency.FullName;
                    this._dependendOnMethod = attribute.MethodDependency;
                }
            }

            public string TestType => this.TestCase.TestMethod.TestClass.Class.Name;
            public string TestMethod => this.TestCase.TestMethod.Method.Name;

            public ITestCase TestCase { get; }

            public bool IsDependency(TestCaseWrapper other) {
                if (this._dependendOnMethod == null || this._dependendOnType == null) {
                    return false;
                }

                var dependendOnType = string.Equals(other.TestType, this._dependendOnType, StringComparison.Ordinal);
                if (!dependendOnType) {
                    return false;
                }

                if (this._dependendOnMethod == null) {
                    return true;
                }

                return string.Equals(other.TestMethod, this._dependendOnMethod, StringComparison.Ordinal);
            }

            public override string ToString() => this.TestCase.DisplayName;
        }
    }
}