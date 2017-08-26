using System;
using System.Collections.Generic;
using System.Linq;
using InfluxDB.InfluxQL.Syntax.Expressions;
using InfluxDB.InfluxQL.Syntax.Statements;

namespace InfluxDB.InfluxQL.Tests
{
    public static class QueryExtensions
    {
        public static IEnumerable<string> ColumnNames<TValues>(this SingleSeriesSelectStatement<TValues> query)
        {
            return query.Columns.Select(x =>
            {
                switch (x)
                {
                    case FieldSelectionExpression f:
                        return f.FieldName;
                    case TagSelectionExpression t:
                        return t.TagName;
                    default:
                        throw new NotImplementedException();
                }
            });
        }

        public static IEnumerable<string> TagNames<TValues, TTags>(this MultiSeriesSelectStatement<TValues, TTags> query)
        {
            return query.Tags.Select(x => x.InfluxTagName);
        }
    }
}