namespace NUnit.Extensions.TestOrdering {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using NUnit.Framework;
    using NUnit.Framework.Internal;

    public class TestFixtureDependencyChainer {
        public static TestFixtureDependencyChainer Instance = new TestFixtureDependencyChainer();

        private readonly List<TestCaseWrapper> _testCaseDescriptors = new List<TestCaseWrapper>();

        private TestFixtureDependencyChainer() {
            var types = 
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetExportedTypes()
                where type.GetCustomAttribute<TestFixtureAttribute>() != null
                select new TestCaseWrapper(new TestFixtureDescriptor(type));

            this._testCaseDescriptors.AddRange(DependencySorter.Sort(types));
        }

        public int GetOrder(Test test) {
            TestFixtureDescriptor testFixtureDescriptor = new TestFixtureDescriptor(test.TypeInfo.Type);

            return this._testCaseDescriptors.FindIndex(x => x.Descriptor == testFixtureDescriptor);
        }

        private sealed class TestFixtureDescriptor : IEquatable<TestFixtureDescriptor> {
            private readonly string _assembly;
            private readonly string _type;

            public string Assembly => this._assembly;
            public string Type => this._type;

            public TestFixtureDescriptor(Type dependendOnType) {
                this._type = dependendOnType.FullName;
                this._assembly = dependendOnType.Assembly.FullName;
            }

            public bool Equals(TestFixtureDescriptor other) {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(this._type, other._type);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is TestFixtureDescriptor && this.Equals((TestFixtureDescriptor) obj);
            }

            public override int GetHashCode() {
                return this._type?.GetHashCode() ?? 0;
            }

            public static bool operator ==(TestFixtureDescriptor left, TestFixtureDescriptor right) {
                if (ReferenceEquals(null, left)) return ReferenceEquals(null, right);
                if (ReferenceEquals(null, right)) return false;

                return left.Equals(right);
            }

            public static bool operator !=(TestFixtureDescriptor left, TestFixtureDescriptor right) {
                return !(left == right);
            }

            public override string ToString() => $"{this.Type}";
        }

        private sealed class TestCaseWrapper : IDependencyIndicator<TestCaseWrapper> {
            public TestCaseWrapper(TestFixtureDescriptor testFixture) {
                this.Descriptor = testFixture;

                var type = Assembly.Load(testFixture.Assembly).GetType(testFixture.Type, true);

                TestFixtureDependencyAttribute attribute = type.GetCustomAttribute<TestFixtureDependencyAttribute>();

                if (attribute != null) {
                    this.Dependency = new TestFixtureDescriptor(attribute.FixtureDependency);
                }
            }


            public TestFixtureDescriptor Descriptor { get; }
            public TestFixtureDescriptor Dependency { get; }

            public bool IsDependencyOf(TestCaseWrapper other) {
                if (other.Dependency == null) {
                    return false;
                }

                return this.Descriptor == other.Dependency;
            }

            public bool HasDependencies => this.Dependency != null;

            public bool Equals(TestCaseWrapper other) {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.Descriptor, other.Descriptor);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is TestCaseWrapper && this.Equals((TestCaseWrapper) obj);
            }

            public override int GetHashCode() {
                return this.Descriptor?.GetHashCode() ?? 0;
            }

            public override string ToString() => this.Descriptor.ToString();
        }
    }

    
}