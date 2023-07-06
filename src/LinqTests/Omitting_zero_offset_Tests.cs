using System.Linq;
using Marten;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Shouldly;

namespace LinqTests;

public class Omitting_zero_offset_Tests : IntegrationContext
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(10, false)]
    public void sql_command_should_not_contain_OFFSET_with_zero_value(int skipCount, bool omit)
    {
        // given
        var queryable = theSession
            .Query<Target>()
            .Skip(skipCount);

        // when
        var sql = queryable.ToCommand().CommandText;

        // than
        if (omit)
        {
            sql.ShouldNotContain("OFFSET :", Case.Insensitive);
        }
        else
        {
            sql.ShouldContain("OFFSET :", Case.Insensitive);
        }
    }

    public Omitting_zero_offset_Tests(DefaultStoreFixture fixture) : base(fixture)
    {
    }
}