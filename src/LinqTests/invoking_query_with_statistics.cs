using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Marten;
using Marten.Linq;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Shouldly;

namespace LinqTests;

public class invoking_query_with_statistics: IntegrationContext
{
    public invoking_query_with_statistics(DefaultStoreFixture fixture) : base(fixture)
    {

    }

    protected override Task fixtureSetup()
    {
        return theStore.BulkInsertAsync(Target.GenerateRandomData(100).ToArray());
    }

    #region sample_compiled-query-statistics
    public class TargetPaginationQuery: ICompiledListQuery<Target>
    {
        public TargetPaginationQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public QueryStatistics Stats { get; } = new QueryStatistics();

        public Expression<Func<IMartenQueryable<Target>, IEnumerable<Target>>> QueryIs()
        {
            return query => query
                .Where(x => x.Number > 10)
                .Skip(PageNumber)
                .Take(PageSize);
        }
    }

    #endregion

    [Fact]
    public void can_get_the_total_from_a_compiled_query()
    {
        var count = theSession.Query<Target>().Count(x => x.Number > 10);
        count.ShouldBeGreaterThan(0);

        var query = new TargetPaginationQuery(2, 5);
        var list = theSession
            .Query(query)
            .ToList();

        list.Any().ShouldBeTrue();

        query.Stats.TotalResults.ShouldBe(count);
    }

    [Fact]
    public async Task can_use_json_streaming_with_statistics()
    {

        var count = theSession.Query<Target>().Count(x => x.Number > 10);
        count.ShouldBeGreaterThan(0);

        var query = new TargetPaginationQuery(2, 5);
        var stream = new MemoryStream();
        var resultCount = await theSession
            .StreamJsonMany(query, stream);

        resultCount.ShouldBeGreaterThan(0);

        stream.Position = 0;
        var list = theStore.Options.Serializer().FromJson<Target[]>(stream);
        list.Length.ShouldBe(5);

    }

    [Fact]
    public async Task can_get_the_total_from_a_compiled_query_running_in_a_batch()
    {
        var count = await theSession.Query<Target>().Where(x => x.Number > 10).CountAsync();
        SpecificationExtensions.ShouldBeGreaterThan(count, 0);

        var query = new TargetPaginationQuery(2, 5);

        var batch = theSession.CreateBatchQuery();

        var targets = batch.Query(query);

        await batch.Execute();

        (await targets)
            .Any().ShouldBeTrue();

        query.Stats.TotalResults.ShouldBe(count);
    }

    [Fact]
    public void can_get_the_total_from_a_compiled_query_running_in_a_batch_sync()
    {
        var count = theSession.Query<Target>().Count(x => x.Number > 10);
        SpecificationExtensions.ShouldBeGreaterThan(count, 0);

        var query = new TargetPaginationQuery(2, 5);

        var batch = theSession.CreateBatchQuery();

        var targets = batch.Query(query);

        batch.ExecuteSynchronously();

        targets.Result
            .Any().ShouldBeTrue();

        query.Stats.TotalResults.ShouldBe(count);
    }

    [Fact]
    public async Task can_get_the_total_in_batch_query()
    {
        var count = await theSession.Query<Target>().Where(x => x.Number > 10).CountAsync();
        SpecificationExtensions.ShouldBeGreaterThan(count, 0);

        QueryStatistics stats = null;

        var batch = theSession.CreateBatchQuery();

        var list = batch.Query<Target>().Stats(out stats).Where(x => x.Number > 10).Take(5)
            .ToList();

        await batch.Execute();

        (await list).Any().ShouldBeTrue();

        stats.TotalResults.ShouldBe(count);
    }

    [Fact]
    public void can_get_the_total_in_batch_query_sync()
    {
        var count = theSession.Query<Target>().Count(x => x.Number > 10);
        SpecificationExtensions.ShouldBeGreaterThan(count, 0);

        QueryStatistics stats = null;

        var batch = theSession.CreateBatchQuery();

        var list = batch.Query<Target>().Stats(out stats).Where(x => x.Number > 10).Take(5)
            .ToList();

        batch.ExecuteSynchronously();

        list.Result.Any().ShouldBeTrue();

        stats.TotalResults.ShouldBe(count);
    }

    #region sample_using-query-statistics
    [Fact]
    public void can_get_the_total_in_results()
    {
        var count = theSession.Query<Target>().Count(x => x.Number > 10);
        SpecificationExtensions.ShouldBeGreaterThan(count, 0);

        // We're going to use stats as an output
        // parameter to the call below, so we
        // have to declare the "stats" object
        // first
        QueryStatistics stats = null;

        var list = theSession
            .Query<Target>()
            .Stats(out stats)
            .Where(x => x.Number > 10).Take(5)
            .ToList();

        list.Any().ShouldBeTrue();

        // Now, the total results data should
        // be available
        stats.TotalResults.ShouldBe(count);
    }

    #endregion

    [Fact]
    public async Task can_get_the_total_in_results_async()
    {
        var count = await theSession.Query<Target>().Where(x => x.Number > 10).CountAsync();
        SpecificationExtensions.ShouldBeGreaterThan(count, 0);

        QueryStatistics stats = null;

        var list = await theSession.Query<Target>().Stats(out stats).Where(x => x.Number > 10).Take(5)
            .ToListAsync();

        list.Any().ShouldBeTrue();

        stats.TotalResults.ShouldBe(count);
    }
}