using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Shouldly;

namespace LinqTests;

public class dictionary_is_translated: IntegrationContext
{
    public dictionary_is_translated(DefaultStoreFixture fixture) : base(fixture)
    {
        theStore.BulkInsert(Target.GenerateRandomData(100).ToArray());
    }

    [Fact]
    public void dictionary_string_containskey_is_translated_to_json_map()
    {
        var query = theSession.Query<Target>().Where(t => t.StringDict.ContainsKey("foo"));
        var command = query.ToCommand(Marten.Linq.FetchType.FetchMany);
        var dictParam = command.Parameters[0];
        (dictParam.DbType == System.Data.DbType.String).ShouldBeTrue();
        (dictParam.Value.ToString() == "foo").ShouldBeTrue();
    }

    [Fact]
    public void dictionary_guid_containskey_is_translated_to_json_map()
    {
        var guid = Guid.NewGuid();
        var query = theSession.Query<Target>().Where(t => t.GuidDict.ContainsKey(guid));
        var command = query.ToCommand(Marten.Linq.FetchType.FetchMany);
        var dictParam = command.Parameters[0];
        (dictParam.DbType == System.Data.DbType.String).ShouldBeTrue();
        (dictParam.Value.ToString() == guid.ToString()).ShouldBeTrue();
    }

    // using key0 and value0 for these because the last node, which is deep, should have at least a single dict node

    [Fact]
    public void dict_string_can_query_using_containskey()
    {
        var results = theSession.Query<Target>().Where(x => x.StringDict.ContainsKey("key0")).ToList();
        results.All(r => r.StringDict.ContainsKey("key0")).ShouldBeTrue();
    }

    [Fact]
    public async Task dict_guid_can_query_using_containskey()
    {
        var guid = Guid.NewGuid();
        var target = new Target();
        target.GuidDict.Add(guid, Guid.NewGuid());
        theSession.Store(target);
        await theSession.SaveChangesAsync();

        var results = await theSession.Query<Target>().Where(x => x.GuidDict.ContainsKey(guid)).ToListAsync();
        results.All(r => r.GuidDict.ContainsKey(guid)).ShouldBeTrue();
    }

    [Fact]
    public void dict_string_can_query_using_containsKVP()
    {
        var kvp = new KeyValuePair<string, string>("key0", "value0");
        var results = theSession.Query<Target>().Where(x => x.StringDict.Contains(kvp)).ToList();
        results.All(r => r.StringDict.Contains(kvp)).ShouldBeTrue();
    }

    [Fact]
    public async Task dict_guid_can_query_using_containsKVP()
    {
        var guidk = Guid.NewGuid();
        var guidv = Guid.NewGuid();
        var target = new Target();
        target.GuidDict.Add(guidk, guidv);
        theSession.Store(target);
        await theSession.SaveChangesAsync();

        var kvp = new KeyValuePair<Guid, Guid>(guidk, guidv);
        // Only works if the dictionary is in interface form
        var results = await theSession.Query<Target>().Where(x => ((IDictionary<Guid, Guid>)x.GuidDict).Contains(kvp)).ToListAsync();
        results.All(r => r.GuidDict.Contains(kvp)).ShouldBeTrue();
    }

    [Fact]
    public void icollection_keyvaluepair_contains_is_translated_to_json_map()
    {
        var query = theSession.Query<Target>().Where(t => t.StringDict.Contains(new KeyValuePair<string, string>("foo", "bar")));
        var command = query.ToCommand(Marten.Linq.FetchType.FetchMany);
        var dictParam = command.Parameters[0];
        (dictParam.NpgsqlDbType == NpgsqlTypes.NpgsqlDbType.Jsonb).ShouldBeTrue();
        (dictParam.Value.ToString() == "{\"foo\":\"bar\"}").ShouldBeTrue();
    }
}
