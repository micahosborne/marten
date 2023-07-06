using System.Linq;
using System.Threading.Tasks;
using Marten;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Shouldly;

namespace LinqTests.Operators;

public class sum_operator: IntegrationContext
{

    [Fact]
    public void sum_without_any_where()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        theSession.SaveChanges();

        var result = theSession.Query<Target>().Sum(x => x.Number);
        result.ShouldBe(10);
    }

    [Fact]
    public void sum_with_nullable()
    {
        theSession.Store(new Target { NullableNumber = 1 });
        theSession.Store(new Target { NullableNumber = 2 });
        theSession.Store(new Target { NullableNumber = 3 });
        theSession.Store(new Target { NullableNumber = 4 });
        theSession.SaveChanges();

        var result = theSession.Query<Target>().Sum(x => x.NullableNumber);
        result.ShouldBe(10);
    }


    [Fact]
    public async Task sum_without_any_where_async()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        await theSession.SaveChangesAsync();

        var result = await theSession.Query<Target>().SumAsync(x => x.Number);
        result.ShouldBe(10);
    }

    [Fact]
    public async Task sum_with_nullable_async()
    {
        theSession.Store(new Target { NullableNumber = 1 });
        theSession.Store(new Target { NullableNumber = 2 });
        theSession.Store(new Target { NullableNumber = 3 });
        theSession.Store(new Target { NullableNumber = 4 });
        await theSession.SaveChangesAsync();

        var result = await theSession.Query<Target>().SumAsync(x => x.NullableNumber);
        result.ShouldBe(10);
    }

    public sum_operator(DefaultStoreFixture fixture) : base(fixture)
    {
    }
}
