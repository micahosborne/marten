﻿using System.Linq;
using Marten;
using Marten.Linq;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Shouldly;
using Xunit;

namespace DocumentDbTests.Reading.Linq;

public class select_transformations_Tests : IntegrationContext
{
    [Fact]
    public void build_query_for_a_single_field()
    {
        SpecificationExtensions.ShouldBeNull(theSession.Query<User>().Select(x => x.UserName).FirstOrDefault());

        var cmd = theSession.Query<User>().Select(x => x.UserName).ToCommand(FetchType.FetchMany);

        cmd.CommandText.ShouldBe("select d.data ->> 'UserName' from public.mt_doc_user as d");
    }

    public select_transformations_Tests(DefaultStoreFixture fixture) : base(fixture)
    {
    }
}

public class select_transformations_with_database_schema_Tests : OneOffConfigurationsContext
{

    [Fact]
    public void build_query_for_a_single_field()
    {
        StoreOptions(_ => _.DatabaseSchemaName = "other_select");

        SpecificationExtensions.ShouldBeNull(theSession.Query<User>().Select(x => x.UserName).FirstOrDefault());

        var cmd = theSession.Query<User>().Select(x => x.UserName).ToCommand(FetchType.FetchMany);

        cmd.CommandText.ShouldBe("select d.data ->> 'UserName' from other_select.mt_doc_user as d");
    }


}