namespace BankAccountApp.NUnitTests.Integration.Extensions {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using NUnit.Framework;
    using NUnit.Framework.Internal;

    public class TestDependencyChainer {
        public static TestDependencyChainer Instance = new TestDependencyChainer();

        private readonly List<TestCaseDescriptor> _testCaseDescriptors = new List<TestCaseDescriptor>();

        private TestDependencyChainer() {
            Debugger.Break();

            var excludedMethodNames = new[] {nameof(this.GetHashCode), nameof(Equals), nameof(this.ToString), nameof(this.GetType)};
            var types = from type in typeof(TestDependencyChainer).Assembly.GetExportedTypes()
                where type.GetCustomAttribute<TestFixtureAttribute>() != null
                from method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                where Array.IndexOf(excludedMethodNames, method.Name) == -1
                select new TestCaseWrapper(new TestCaseDescriptor(method.Name, type));

            this._testCaseDescriptors.AddRange(
                types.OrderBy(x => x, new TestCaseComparer()).Select(x => x.Descriptor));
        }

        public int GetOrder(Test test) {
            TestCaseDescriptor testCaseDescriptor = new TestCaseDescriptor(test.Method.Name, test.Method.TypeInfo.Type);

            return this._testCaseDescriptors.FindIndex(x => x == testCaseDescriptor);
        }
    }


    internal sealed class TestCaseComparer : Comparer<TestCaseWrapper> {
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

            if (y.IsDependency(x.Descriptor)) {
                return -1; // x is less than y
            }

            if (x.IsDependency(y.Descriptor)) {
                return 1; // y is less than x
            }

            // Follow alphabetic ordering
            var strComparison = StringComparer.Ordinal.Compare(x.Descriptor.Type, y.Descriptor.Type);
            if (strComparison == 0) strComparison = StringComparer.Ordinal.Compare(x.Descriptor.Method, y.Descriptor.Method);

            return strComparison;
        }
    }

    internal sealed class TestCaseDescriptor : IEquatable<TestCaseDescriptor> {
        private readonly string _method;
        private readonly string _type;

        public string Method => this._method;
        public string Type => this._type;

        public TestCaseDescriptor(string method, Type dependendOnType) {
            this._method = method;
            this._type = dependendOnType.FullName;
        }

        public bool Equals(TestCaseDescriptor other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(this._method, other._method) && string.Equals(this._type, other._type);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is TestCaseDescriptor && this.Equals((TestCaseDescriptor) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((this._method != null ? this._method.GetHashCode() : 0)*397) ^ (this._type != null ? this._type.GetHashCode() : 0);
            }
        }

        public static bool operator ==(TestCaseDescriptor left, TestCaseDescriptor right) {
            return Equals(left, right);
        }

        public static bool operator !=(TestCaseDescriptor left, TestCaseDescriptor right) {
            return !Equals(left, right);
        }

        public override string ToString() => $"{this.Type}.{this.Method}";
    }

    internal sealed class TestCaseWrapper {
        public TestCaseWrapper(TestCaseDescriptor testCase) {
            this.Descriptor = testCase;

            var type = typeof(TestCaseWrapper).Assembly.GetType(testCase.Type, true);

            MethodInfo method = null;
            if (testCase.Method != null) {
                method = type.GetMethod(testCase.Method, BindingFlags.Public | BindingFlags.Instance);
                if (method == null) throw new InvalidOperationException($"Unable to find method '{testCase.Method}' on {type}");
            }

            TestDependencyAttribute attribute = method.GetCustomAttribute<TestDependencyAttribute>();

            if (attribute != null) {
                this.Dependency = new TestCaseDescriptor(
                    attribute.MethodDependency,
                    attribute.TypeDependency ?? type);
            }
        }


        public TestCaseDescriptor Descriptor { get; }
        public TestCaseDescriptor Dependency { get; }

        public bool IsDependency(TestCaseDescriptor other) {
            if (other == null) {
                return false;
            }

            if (this.Dependency == null) {
                return false;
            }

            bool dependendOnType = string.Equals(other.Type, this.Dependency.Type, StringComparison.Ordinal);
            if (!dependendOnType) {
                return false;
            }

            if (this.Dependency.Method == null) {
                return true;
            }

            return string.Equals(other.Method, this.Dependency.Method, StringComparison.Ordinal);
        }

        public override string ToString() => this.Descriptor.ToString();
    }
}