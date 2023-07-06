﻿using System.Linq;
using System.Threading.Tasks;
using Marten;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Shouldly;

namespace LinqTests;

public class query_with_aggregate_functions : IntegrationContext
{
    #region sample_using_max
    [Fact]
    public void get_max()
    {
        theSession.Store(new Target { Color = Colors.Blue, Number = 1 });
        theSession.Store(new Target { Color = Colors.Red, Number = 42 });
        theSession.Store(new Target { Color = Colors.Green, Number = 3 });
        theSession.Store(new Target { Color = Colors.Blue, Number = 4 });

        theSession.SaveChanges();
        var maxNumber = theSession.Query<Target>().Max(t => t.Number);
        maxNumber.ShouldBe(42);
    }
    #endregion

    [Fact]
    public async Task get_max_async()
    {
        theSession.Store(new Target { Color = Colors.Blue, Number = 1 });
        theSession.Store(new Target { Color = Colors.Red, Number = 42 });
        theSession.Store(new Target { Color = Colors.Green, Number = 3 });
        theSession.Store(new Target { Color = Colors.Blue, Number = 4 });

        theSession.SaveChanges();
        var maxNumber = await theSession.Query<Target>().MaxAsync(t => t.Number);
        maxNumber.ShouldBe(42);
    }

    #region sample_using_min
    [Fact]
    public void get_min()
    {
        theSession.Store(new Target { Color = Colors.Blue, Number = 1 });
        theSession.Store(new Target { Color = Colors.Red, Number = 2 });
        theSession.Store(new Target { Color = Colors.Green, Number = -5 });
        theSession.Store(new Target { Color = Colors.Blue, Number = 42 });

        theSession.SaveChanges();
        var minNumber = theSession.Query<Target>().Min(t => t.Number);
        minNumber.ShouldBe(-5);
    }
    #endregion

    [Fact]
    public async Task get_min_async()
    {
        theSession.Store(new Target { Color = Colors.Blue, Number = 1 });
        theSession.Store(new Target { Color = Colors.Red, Number = 42 });
        theSession.Store(new Target { Color = Colors.Green, Number = -5 });
        theSession.Store(new Target { Color = Colors.Blue, Number = 4 });

        await theSession.SaveChangesAsync();
        var maxNumber = await theSession.Query<Target>().MinAsync(t => t.Number);
        maxNumber.ShouldBe(-5);
    }

    #region sample_using_average
    [Fact]
    public void get_average()
    {
        theSession.Store(new Target { Color = Colors.Blue, Number = 1 });
        theSession.Store(new Target { Color = Colors.Red, Number = 2 });
        theSession.Store(new Target { Color = Colors.Green, Number = -5 });
        theSession.Store(new Target { Color = Colors.Blue, Number = 42 });

        theSession.SaveChanges();
        var average = theSession.Query<Target>().Average(t => t.Number);
        average.ShouldBe(10);
    }
    #endregion

    [Fact]
    public async Task get_average_async()
    {
        theSession.Store(new Target { Color = Colors.Blue, Number = 1 });
        theSession.Store(new Target { Color = Colors.Red, Number = 42 });
        theSession.Store(new Target { Color = Colors.Green, Number = -5 });
        theSession.Store(new Target { Color = Colors.Blue, Number = 2 });

        await theSession.SaveChangesAsync();
        var maxNumber = await theSession.Query<Target>().AverageAsync(t => t.Number);
        maxNumber.ShouldBe(10);
    }

    public query_with_aggregate_functions(DefaultStoreFixture fixture) : base(fixture)
    {
    }
}