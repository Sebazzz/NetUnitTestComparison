# The .NET Test Framework comparison

Comparison between MSTest 10.0 (VS2015), NUnit 3.2.1 and XUnit.NET 2.1. This repository is accompanied by [a blog post](http://damsteen.nl/blog/2016/06/05/ordered-tests-with-nunit-mstest-xunit-pt1).

Run tests using:

- Powershell scripts: `Run-UnitTestComparison.{TEST FRAMEWORK}.ps1`
- ReSharper test runner
- Visual Studio test runner

Below the test types are described.

## Plain unit test
There are some plain unit tests defined in the `BankAccountApp.{TEST FRAMEWORK}.Unit` projects. 

## Basic ordered test using defined dependencies
There are some tests defined in the `BankAccountApp.{TEST FRAMEWORK}.Integration` that are order dependend.  

### NUnit
Current limitation is that only ordering is supported within a `TestFixture` or ordering fixtures in the same namespace. Also, when a test fails, the other tests are happily executed and not ignored.

**Resharper runner:** While the test tree of Resharper does not appear to follow the correct ordering, the tests are actually performed in the correct order.

**Visual Studio runner:** The tests are not correctly ordered in the Test Explorer, but the tests themselves execute in the correct order. When grouping tests by trait, the test fixtures are shown in the correct order but the underlying tests are not.

**Console runner:** Shows (and executes) the tests in the correct order.

### XUnit
Each test class needs to be put in an unique test collection. Test collections can be ordered against each other. Within a test collection, tests can be ordered. When a test fails, the other tests are happily execute and not ignored.

**Resharper runner:** While the test tree of Resharper does not appear to follow the correct ordering, the tests are actually performed in the correct order.

**Visual Studio runner:** The tests are not correctly ordered in the Test Explorer, but the tests themselves execute in the correct order. Grouping tests makes no difference.

**Console runner:** Shows (and executes) the tests in the correct order.

### MSTest
Test ordering is built-in MSTest and works quite good. The only thing to keep aware of is that you need to execute the `.orderedtest` file, and not the individual tests. This also applies to the Test Explorer, where you need to execute the name of the ordered test.

**Resharper runner:** Does not support ordered tests, so ordered tests are shown. Instead, all test methods are shown in hierarchy and on execution correct order is not maintained, as expected.

**Visual Studio runner:** Since the Test Explorer is unable to show hierarchy, the 'child' tests of an ordered tests are not shown. Also, each test method is shown individually in the Test Explorer. When executing tests, the tests are correctly shown at the bottom of the Test Explorer.

**Console runner:** Shows (and executes) the tests in the correct order. Need to make sure the `.orderedtest` file is given on the command line.