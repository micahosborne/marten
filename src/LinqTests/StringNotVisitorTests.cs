﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Xunit.Abstractions;

namespace LinqTests;

public class StringNotVisitorTests : IntegrationContext
{
    private readonly ITestOutputHelper _output;

    public StringNotVisitorTests(DefaultStoreFixture fixture, ITestOutputHelper output) : base(fixture)
    {
        _output = output;

    }

    protected override Task fixtureSetup()
    {
        var entry = new User() { FirstName = "Beeblebrox" };
        var entry2 = new User() { FirstName = "Bee" };
        var entry3 = new User() { FirstName = "Zaphod" };
        var entry4 = new User() { FirstName = "Zap" };

        return theStore.BulkInsertAsync(new[] { entry, entry2, entry3, entry4 });
    }

    [Theory]
    [InlineData("zap", StringComparison.OrdinalIgnoreCase, 3)]
    [InlineData("Zap", StringComparison.CurrentCulture, 3)]
    [InlineData("zap", StringComparison.CurrentCulture, 4)]
    public void CanQueryByNotEquals(string search, StringComparison comparison, int expectedCount)
    {
        using var s = theStore.QuerySession();
        var fromDb = s.Query<User>().Where(x => !x.FirstName.Equals(search, comparison)).ToList();
        Assert.Equal(expectedCount, fromDb.Count);
    }

    [Theory]
    [InlineData("zap", StringComparison.OrdinalIgnoreCase, 2)]
    [InlineData("zap", StringComparison.CurrentCulture, 4)]
    public void CanQueryByNotContains(string search, StringComparison comparison, int expectedCount)
    {
        using var s = theStore.QuerySession();
        var fromDb = s.Query<User>().Where(x => !x.FirstName.Contains(search, comparison)).ToList();
        Assert.Equal(expectedCount, fromDb.Count);
    }

    [Theory]
    [InlineData("zap", StringComparison.OrdinalIgnoreCase, 2)]
    [InlineData("zap", StringComparison.CurrentCulture, 4)]
    public void CanQueryByNotStartsWith(string search, StringComparison comparison, int expectedCount)
    {
        using var s = theStore.QuerySession();
        var fromDb = s.Query<User>().Where(x => !x.FirstName.StartsWith(search, comparison)).ToList();
        Assert.Equal(expectedCount, fromDb.Count);
    }

    [Theory]
    [InlineData("hod", StringComparison.OrdinalIgnoreCase, 3)]
    [InlineData("HOD", StringComparison.OrdinalIgnoreCase, 3)]
    [InlineData("Hod", StringComparison.CurrentCulture, 4)]
    public void CanQueryByNotEndsWith(string search, StringComparison comparison, int expectedCount)
    {
        using var s = theStore.QuerySession();
        var fromDb = s.Query<User>().Where(x => !x.FirstName.EndsWith(search, comparison)).ToList();
        Assert.Equal(expectedCount, fromDb.Count);
    }

    [Theory]
    [InlineData("zap", "hod", StringComparison.OrdinalIgnoreCase, 1)]
    [InlineData("zap", "hod", StringComparison.CurrentCulture, 0)]
    public void CanMixContainsAndNotContains(string contains, string notContains, StringComparison comparison, int expectedCount)
    {
        using var s = theStore.QuerySession();
        var fromDb = s.Query<User>().Where(x => !x.FirstName.Contains(notContains, comparison) && x.FirstName.Contains(contains, comparison)).ToList();
        Assert.Equal(expectedCount, fromDb.Count);
    }
}
