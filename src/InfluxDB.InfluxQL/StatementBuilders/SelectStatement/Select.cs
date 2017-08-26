using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using InfluxDB.InfluxQL.Schema;
using InfluxDB.InfluxQL.Syntax;
using InfluxDB.InfluxQL.Syntax.Clauses;
using InfluxDB.InfluxQL.Syntax.Expressions;
using InfluxDB.InfluxQL.Syntax.Statements;

namespace InfluxDB.InfluxQL.StatementBuilders.SelectStatement
{
    public class Select<TValues, TFields>
    {
        private readonly Measurement measurement;
        private readonly SelectClause select;
        private readonly FromClause from;

        internal Select(Measurement measurement, FromClause from, LambdaExpression selectExpression)
        {
            this.measurement = measurement ?? throw new ArgumentNullException(nameof(measurement));
            this.from = from ?? throw new ArgumentNullException(nameof(from));

            if (selectExpression == null)
            {
                throw new ArgumentNullException(nameof(selectExpression));
            }

            var columns = ParseExpression(selectExpression).ToArray();

            this.select = new SelectClause(columns);
        }

        public SingleSeriesSelectStatement<TValues> Statement => new SingleSeriesSelectStatement<TValues>(select, from);

        public Where<TValues> Where(string predicate)
        {
            return new Where<TValues>(select, from, predicate);
        }

        public override string ToString()
        {
            return Statement.Text;
        }

        public static implicit operator SingleSeriesSelectStatement<TValues>(Select<TValues, TFields> builder)
        {
            return builder.Statement;
        }

        private IEnumerable<ColumnSelectionExpression> ParseExpression(LambdaExpression selectExpression)
        {
            Expression fieldsParameter = selectExpression.Parameters[0];

            if (selectExpression.Body == fieldsParameter)
            {
                // Identity function so all fields selected.
                return measurement.FieldSet.Select(x => new FieldSelectionExpression(x.InfluxFieldName, x.DotNetAlias));
            }

            switch (selectExpression.Body.NodeType)
            {
                case ExpressionType.New:
                    return ParseProjection((NewExpression)selectExpression.Body, fieldsParameter);
                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerable<ColumnSelectionExpression> ParseProjection(NewExpression expression, Expression fieldsParameter)
        {
            for (var i = 0; i < expression.Arguments.Count; i++)
            {
                MemberInfo member = expression.Members[i];
                var argument = expression.Arguments[i];

                switch (argument.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        yield return ParseColumnSelection((MemberExpression)argument, member, fieldsParameter);
                        break;
                    case ExpressionType.Call:
                        var call = ((MethodCallExpression)argument);
                        var fieldSelection = ParseColumnSelection((MemberExpression)call.Arguments[0], member, fieldsParameter);
                        var aggregation = new FieldAggregationExpression(call.Method.Name);

                        yield return new FieldSelectionExpression(fieldSelection, aggregation);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private FieldSelectionExpression ParseColumnSelection(MemberExpression argument, MemberInfo member, Expression fieldsParameter)
        {
            if (argument.Expression == fieldsParameter)
            {
                var field = measurement.FieldSet.Single(x => x.DotNetAlias == argument.Member.Name);

                return new FieldSelectionExpression(field.InfluxFieldName, member.Name);
            }

            throw new NotImplementedException();
        }
    }

    public class Select<TValues, TFields, TTags>
    {
        private readonly TaggedMeasurement measurement;
        private readonly SelectClause select;
        private readonly FromClause from;

        internal Select(TaggedMeasurement measurement, FromClause from, LambdaExpression selectExpression)
        {
            this.measurement = measurement ?? throw new ArgumentNullException(nameof(measurement));
            this.from = from ?? throw new ArgumentNullException(nameof(from));

            if (selectExpression == null)
            {
                throw new ArgumentNullException(nameof(selectExpression));
            }

            var columns = ParseExpression(selectExpression).ToImmutableList();

            this.select = new SelectClause(columns);
        }

        public SingleSeriesSelectStatement<TValues> Statement => new SingleSeriesSelectStatement<TValues>(select, from);

        public Where<TValues, TTags> Where(string predicate)
        {
            return new Where<TValues, TTags>(measurement, select, from, predicate);
        }

        public GroupBy<TValues> GroupBy(TimeSpan timeInterval)
        {
            return new GroupBy<TValues>(select, from, timeInterval);
        }

        public GroupBy<TValues, TGroupBy> GroupBy<TGroupBy>(Expression<Func<TTags, TGroupBy>> tagSelection)
        {
            return new GroupBy<TValues, TGroupBy>(measurement, select, from, tagSelection);
        }

        public GroupBy<TValues, TGroupBy> GroupBy<TGroupBy>(TimeSpan timeInterval, Expression<Func<TTags, TGroupBy>> tagSelection)
        {
            return new GroupBy<TValues, TGroupBy>(measurement, select, from, tagSelection, timeInterval: timeInterval);
        }

        public override string ToString()
        {
            return Statement.Text;
        }

        public static implicit operator SingleSeriesSelectStatement<TValues>(Select<TValues, TFields, TTags> builder)
        {
            return builder.Statement;
        }

        private IEnumerable<ColumnSelectionExpression> ParseExpression(LambdaExpression selectExpression)
        {
            Expression fieldsParameter = selectExpression.Parameters[0];
            Expression tagsParameter = null;
            if (selectExpression.Parameters.Count == 2)
            {
                tagsParameter = selectExpression.Parameters[1];
            }

            if (selectExpression.Body == fieldsParameter)
            {
                // Identity function so all fields selected.
                return measurement.FieldSet.Select(x => new FieldSelectionExpression(x.InfluxFieldName, x.DotNetAlias));
            }

            switch (selectExpression.Body.NodeType)
            {
                case ExpressionType.New:
                    return ParseProjection((NewExpression)selectExpression.Body, fieldsParameter, tagsParameter);
                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerable<ColumnSelectionExpression> ParseProjection(NewExpression expression, Expression fieldsParameter, Expression tagsParameter)
        {
            for (var i = 0; i < expression.Arguments.Count; i++)
            {
                var member = expression.Members[i];
                var argument = expression.Arguments[i];

                switch (argument.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        if (((MemberExpression)argument).Expression == fieldsParameter)
                        {
                            yield return ParseFieldSelection((MemberExpression)argument, member, fieldsParameter);
                        }
                        else if (((MemberExpression)argument).Expression == tagsParameter)
                        {
                            var tag = measurement.TagSet.Single(x => x.DotNetAlias == ((MemberExpression)argument).Member.Name);

                            yield return new TagSelectionExpression(tag.InfluxTagName, member.Name);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        break;
                    case ExpressionType.Call:
                        var call = ((MethodCallExpression)argument);
                        var fieldSelection = ParseFieldSelection((MemberExpression)call.Arguments[0], member, fieldsParameter);
                        var aggregation = new FieldAggregationExpression(call.Method.Name);

                        yield return new FieldSelectionExpression(fieldSelection, aggregation);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private FieldSelectionExpression ParseFieldSelection(MemberExpression argument, MemberInfo member, Expression fieldsParameter)
        {
            if (argument.Expression == fieldsParameter)
            {
                var field = measurement.FieldSet.Single(x => x.DotNetAlias == argument.Member.Name);

                return new FieldSelectionExpression(field.InfluxFieldName, member.Name);
            }

            throw new NotImplementedException();
        }
    }
}