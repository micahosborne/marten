using System.Linq;
using System.Threading.Tasks;
using Marten;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Shouldly;

namespace LinqTests;

public class invoking_queryable_any_async_Tests: IntegrationContext
{
    [Fact]
    public async Task any_miss_with_query()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        await theSession.SaveChangesAsync();

        var result = await theSession.Query<Target>().AnyAsync(x => x.Number == 11);
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task naked_any_miss()
    {
        var result = await theSession.Query<Target>().AnyAsync();
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task naked_any_hit()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        theSession.SaveChanges();

        var result = await theSession.Query<Target>().AnyAsync();
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task any_hit_with_only_one_document()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        theSession.SaveChanges();

        var result = await theSession.Query<Target>().AnyAsync(x => x.Number == 3);
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task any_hit_with_more_than_one_match()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 4 });
        theSession.SaveChanges();

        var result = await theSession.Query<Target>().Where(x => x.Number == 2).AnyAsync();
        result.ShouldBeTrue();
    }

    public invoking_queryable_any_async_Tests(DefaultStoreFixture fixture) : base(fixture)
    {
    }
}