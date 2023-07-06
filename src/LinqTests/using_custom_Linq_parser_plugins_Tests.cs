﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JasperFx.Core;
using JasperFx.Core.Reflection;
using Marten;
using Marten.Linq.Fields;
using Marten.Linq.Parsing;
using Marten.Testing.Harness;
using Shouldly;
using Weasel.Core;
using Weasel.Postgresql.SqlGeneration;

namespace LinqTests;

public class using_custom_Linq_parser_plugins_Tests
{
    #region sample_using_custom_linq_parser

    [Fact]
    public void query_with_custom_parser()
    {
        using (var store = DocumentStore.For(_ =>
               {
                   _.Connection(ConnectionSource.ConnectionString);

                   // IsBlue is a custom parser I used for testing this
                   _.Linq.MethodCallParsers.Add(new IsBlue());
                   _.AutoCreateSchemaObjects = AutoCreate.All;

                   // This is just to isolate the test
                   _.DatabaseSchemaName = "isblue";
               }))
        {
            store.Advanced.Clean.CompletelyRemoveAll();


            var targets = new List<ColorTarget>();
            for (var i = 0; i < 25; i++)
            {
                targets.Add(new ColorTarget {Color = "Blue"});
                targets.Add(new ColorTarget {Color = "Green"});
                targets.Add(new ColorTarget {Color = "Red"});
            }

            var count = targets.Where(x => x.IsBlue()).Count();

            targets.Each(x => x.Id = Guid.NewGuid());

            store.BulkInsert(targets.ToArray());

            using (var session = store.QuerySession())
            {
                session.Query<ColorTarget>().Count(x => x.IsBlue())
                    .ShouldBe(count);
            }
        }
    }

    #endregion
}

public class ColorTarget
{
    public string Color { get; set; }
    public Guid Id { get; set; }
}

public static class CustomExtensions
{
    #region sample_custom-extension-for-linq

    public static bool IsBlue(this ColorTarget target)
    {
        return target.Color == "Blue";
    }

    #endregion
}

#region sample_IsBlue

public class IsBlue: IMethodCallParser
{
    private static readonly PropertyInfo _property = ReflectionHelper.GetProperty<ColorTarget>(x => x.Color);

    public bool Matches(MethodCallExpression expression)
    {
        return expression.Method.Name == nameof(CustomExtensions.IsBlue);
    }

    public ISqlFragment Parse(IFieldMapping mapping, IReadOnlyStoreOptions options, MethodCallExpression expression)
    {
        var locator = mapping.FieldFor(new MemberInfo[] {_property}).TypedLocator;

        return new WhereFragment($"{locator} = 'Blue'");
    }
}

#endregion
