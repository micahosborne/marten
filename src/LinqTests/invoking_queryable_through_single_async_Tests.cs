using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Marten.Testing.Documents;
using Marten.Testing.Harness;

namespace LinqTests;

public class invoking_queryable_through_single_async_Tests: IntegrationContext
{
    [Fact]
    public async Task single_hit_with_only_one_document()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        await theSession.SaveChangesAsync();

        var target = await theSession.Query<Target>().SingleAsync(x => x.Number == 3);
        target.ShouldNotBeNull();
    }

    [Fact]
    public async Task single_or_default_hit_with_only_one_document()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        await theSession.SaveChangesAsync();

        var target = await theSession.Query<Target>().SingleOrDefaultAsync(x => x.Number == 3);
        target.ShouldNotBeNull();
    }

    [Fact]
    public async Task single_or_default_miss()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        await theSession.SaveChangesAsync();

        var target = await theSession.Query<Target>().SingleOrDefaultAsync(x => x.Number == 11);
        target.ShouldBeNull();
    }

    [Fact]
    public async Task single_hit_with_more_than_one_match()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 4 });
        await theSession.SaveChangesAsync();

        await Exception<InvalidOperationException>.ShouldBeThrownByAsync(async () =>
        {
            await theSession.Query<Target>().Where(x => x.Number == 2).SingleAsync();
        });
    }

    [Fact]
    public async Task single_hit_with_more_than_one_match_and_take_one_should_not_throw()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 4 });
        await theSession.SaveChangesAsync();

        var target = await theSession.Query<Target>().Where(x => x.Number == 2).Take(1).SingleAsync();
        target.ShouldNotBeNull();
    }

    [Fact]
    public async Task single_or_default_hit_with_more_than_one_match()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 4 });
        await theSession.SaveChangesAsync();

        await Exception<InvalidOperationException>.ShouldBeThrownByAsync(async () =>
        {
            await theSession.Query<Target>().Where(x => x.Number == 2).SingleOrDefaultAsync();
        });
    }

    [Fact]
    public async Task single_miss()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        await theSession.SaveChangesAsync();

        await Exception<InvalidOperationException>.ShouldBeThrownByAsync(async () =>
        {
            await theSession.Query<Target>().Where(x => x.Number == 11).SingleAsync();
        });
    }

    public invoking_queryable_through_single_async_Tests(DefaultStoreFixture fixture) : base(fixture)
    {
    }
}