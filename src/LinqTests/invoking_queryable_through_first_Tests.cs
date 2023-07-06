using System;
using System.Linq;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Shouldly;
using Xunit;

namespace DocumentDbTests.Reading.Linq;

public class invoking_queryable_through_first_Tests: IntegrationContext
{
    [Fact]
    public void first_hit_with_only_one_document()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        theSession.SaveChanges();

        SpecificationExtensions.ShouldNotBeNull(theSession.Query<Target>().First(x => x.Number == 3));
    }

    [Fact]
    public void first_or_default_hit_with_only_one_document()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        theSession.SaveChanges();

        SpecificationExtensions.ShouldNotBeNull(theSession.Query<Target>().FirstOrDefault(x => x.Number == 3));
    }

    [Fact]
    public void first_or_default_miss()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        theSession.SaveChanges();

        SpecificationExtensions.ShouldBeNull(theSession.Query<Target>().FirstOrDefault(x => x.Number == 11));
    }

    [Fact]
    public void first_correct_hit_with_more_than_one_match()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2, Flag = true });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 4 });
        theSession.SaveChanges();

        theSession.Query<Target>().Where(x => x.Number == 2).First().Flag
            .ShouldBeTrue();
    }

    [Fact]
    public void first_or_default_correct_hit_with_more_than_one_match()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2, Flag = true });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 4 });
        theSession.SaveChanges();

        theSession.Query<Target>().Where(x => x.Number == 2).First().Flag
            .ShouldBeTrue();
    }

    [Fact]
    public void first_miss()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        theSession.SaveChanges();

        Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
        {
            theSession.Query<Target>().Where(x => x.Number == 11).First();
        });
    }

    public invoking_queryable_through_first_Tests(DefaultStoreFixture fixture) : base(fixture)
    {
    }
}