using System;
using System.Linq.Expressions;
using Marten;
using Marten.Linq.SoftDeletes;
using Marten.Schema;
using Marten.Testing.Harness;
using Npgsql;
using Weasel.Postgresql;

namespace LinqTests.SoftDeletes;

public class DeletedBeforeParserTests
{
    private readonly DocumentMapping _mapping;
    private readonly MethodCallExpression _expression;
    private readonly DeletedBeforeParser _parser;
    private readonly StoreOptions _options;

    public DeletedBeforeParserTests()
    {
        _options = new StoreOptions();
        _mapping = new DocumentMapping(typeof(object), _options) { DeleteStyle = DeleteStyle.SoftDelete };
        _expression = Expression.Call(typeof(SoftDeletedExtensions).GetMethod(nameof(SoftDeletedExtensions.DeletedBefore)),
            Expression.Parameter(typeof(object)),
            Expression.Constant(DateTimeOffset.UtcNow));
        _parser = new DeletedBeforeParser();
    }

    [Fact]
    public void WhereFragmentContainsExpectedExpression()
    {
        var result = _parser.Parse(_mapping, _options, _expression);

        var builder = new CommandBuilder(new NpgsqlCommand());

        result.Apply(builder);
        builder.ToString().ShouldContain("d.mt_deleted and d.mt_deleted_at <");
    }

    [Fact]
    public void ThrowsIfDocumentMappingNotSoftDeleted()
    {
        _mapping.DeleteStyle = DeleteStyle.Remove;

        Exception<NotSupportedException>.ShouldBeThrownBy(() => _parser.Parse(_mapping, _options, _expression));
    }
}
