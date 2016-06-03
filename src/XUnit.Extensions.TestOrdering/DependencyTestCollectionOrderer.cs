namespace XUnit.Extensions.TestOrdering {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class DependencyTestCollectionOrderer : ITestCollectionOrderer {
        public const string Name = "XUnit.Extensions.TestOrdering." + nameof(DependencyTestCollectionOrderer);
        public const string Assembly = "XUnit.Extensions.TestOrdering";

        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections) {
            return DependencySorter.Sort(testCollections.Select(x => new TestCollectionWrapper(x))).Select(x => x.TestCollection);
        }

        private sealed class TestCollectionWrapper : IDependencyIndicator<TestCollectionWrapper> {
            private readonly Type _dependendOnType;
            private readonly Type _type;

            public TestCollectionWrapper(ITestCollection testCollection) {
                this.TestCollection = testCollection;

                this._type = TestCollectionCache.Instance.GetType(testCollection.DisplayName);

                TestCollectionDependencyAttribute attribute = this._type?.GetTypeInfo().GetCustomAttribute<TestCollectionDependencyAttribute>();

                if (attribute != null) {
                    this._dependendOnType = attribute.Dependency;
                }
            }

            public string TestType => this._type?.ToString();
            public string TestTypeDependency => this._dependendOnType?.ToString();

            public ITestCollection TestCollection { get; }

            public bool IsDependencyOf(TestCollectionWrapper other) {
                if (other.TestTypeDependency == null) {
                    return false;
                }

                if (this.TestType == null) {
                    return false;
                }

                return string.Equals(this.TestType, other.TestTypeDependency, StringComparison.Ordinal);
            }

            public bool HasDependencies => this._dependendOnType != null;

            public bool Equals(TestCollectionWrapper other) {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.TestCollection, other.TestCollection);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is TestCollectionWrapper && this.Equals((TestCollectionWrapper) obj);
            }

            public override int GetHashCode() {
                return this.TestCollection?.GetHashCode() ?? 0;
            }

            public override string ToString() => this.TestCollection.DisplayName;
        }

        /// <summary>
        /// Helper class for looking up types by collection name
        /// </summary>
        private sealed class TestCollectionCache {
            public static readonly TestCollectionCache Instance = new TestCollectionCache();

            private readonly Dictionary<string, Type> _collectionTypeMap;

            private TestCollectionCache() {
                this._collectionTypeMap = new Dictionary<string, Type>(StringComparer.Ordinal);

                CreateCollectionTypeMap(this._collectionTypeMap);
            }

            private static void CreateCollectionTypeMap(Dictionary<string, Type> collectionTypeMap) {
                var types = 
                    from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetExportedTypes()

                    // Xunit accesses attribute data by constructor arguments
                    let collectionAttrData = type.GetCustomAttributesData().FirstOrDefault(x => x.AttributeType == typeof(CollectionAttribute))
                    where collectionAttrData != null
                    select new {
                        CollectionName = collectionAttrData.ConstructorArguments[0].Value as string,
                        CollectionType = type
                    };

                // We don't use IDictionary because we can't give an useful exception then
                foreach (var tuple in types) {
                    // We don't support null collection names
                    if (tuple.CollectionName == null) continue;

                    try {
                        collectionTypeMap.Add(tuple.CollectionName, tuple.CollectionType);
                    }
                    catch (ArgumentException) {
                        throw new InvalidOperationException(
                            $"Duplicate collection name: {tuple.CollectionName}. Existing collection type with same name: {collectionTypeMap[tuple.CollectionName]}. Trying to add {tuple.CollectionType}");
                    }
                }
            }

            public Type GetType(string collectionName) {
                Type type;
                this._collectionTypeMap.TryGetValue(collectionName, out type);
                return type;
            }
        }
    }
}