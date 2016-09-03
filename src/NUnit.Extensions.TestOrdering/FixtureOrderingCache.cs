namespace NUnit.Extensions.TestOrdering {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Framework;
    using Framework.Internal;

    internal class FixtureOrderingCache {
        public TestCollectionInfo[] TestCollections;

        public FixtureOrderingCache(Assembly targetAssembly) {
            var types =
                from type in targetAssembly.GetExportedTypes()
                where typeof(ITestCollection).IsAssignableFrom(type)
                let testCollectionInstance = (ITestCollection) Reflect.Construct(type)
                select new TestCollectionInfo(testCollectionInstance);

            // Complete the hierarchy
            this.TestCollections = CreateHierarchy(types);
        }

        private static TestCollectionInfo[] CreateHierarchy(IEnumerable<TestCollectionInfo> testCollections) {
            TestCollectionInfo[] returnValue = testCollections.ToArray();

            // O(N*N)
            foreach (TestCollectionInfo testCollection in returnValue) {
                try {
                    testCollection.Parent = returnValue.SingleOrDefault(x => x.IsParentOf(testCollection));
                }
                catch (InvalidOperationException ex) {
                    throw new InvalidOperationException($"Unable to validate hierarchy: test collection {testCollection.TypeName} has multiple parents", ex);
                }
            }

            return returnValue;
        }
    }

    public sealed class TestCollectionInfo : IEquatable<TestCollectionInfo> {
        private readonly Type _testCollectionType;

        public string TypeName => this._testCollectionType.FullName;

        public Type[] ChildTypes { get; }

        public bool ContinueOnFailure { get; }
        public TestCollectionInfo Parent { get; set; }

        public TestCollectionInfo(ITestCollection testCollection) {
            this.ChildTypes = testCollection.GetTestFixtures().ToArray();
            this.ContinueOnFailure = testCollection.ContinueOnFailure;
            this._testCollectionType = testCollection.GetType();
        }

        public bool IsParentOf(TestCollectionInfo other) {
            return this.ChildTypes.Any(x => x == other._testCollectionType);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(TestCollectionInfo other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this._testCollectionType == other._testCollectionType;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is TestCollectionInfo && this.Equals((TestCollectionInfo) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            return this._testCollectionType?.GetHashCode() ?? 0;
        }
    }
}