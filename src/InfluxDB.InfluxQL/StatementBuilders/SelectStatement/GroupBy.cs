using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using InfluxDB.InfluxQL.Schema;
using InfluxDB.InfluxQL.Syntax.Clauses;
using InfluxDB.InfluxQL.Syntax.Literals;
using InfluxDB.InfluxQL.Syntax.Statements;

namespace InfluxDB.InfluxQL.StatementBuilders.SelectStatement
{
    public class GroupBy<TValues>
    {
        private readonly SelectClause select;
        private readonly FromClause from;
        private readonly WhereClause where;
        private readonly GroupByClause groupBy;

        internal GroupBy(SelectClause select, FromClause from, TimeSpan timeInterval, WhereClause where = null)
        {
            this.select = select;
            this.from = from;
            this.where = where;
            this.groupBy = new GroupByClause(new DurationLiteral(timeInterval), new MeasurementTag[0]);
        }

        public SingleSeriesSelectStatement<TValues> Statement => new SingleSeriesSelectStatement<TValues>(select, from, where, groupBy);

        public Fill<TValues> Fill(FillType type) => new Fill<TValues>(Statement, type);

        public OrderBy<TValues> OrderByTimeDesc() => new OrderBy<TValues>(Statement);

        public Limit<TValues> Limit(int n) => new Limit<TValues>(Statement, n);

        public Offset<TValues> Offset(int n) => new Offset<TValues>(Statement, n);

        public override string ToString() => Statement.Text;

        public static implicit operator SingleSeriesSelectStatement<TValues>(GroupBy<TValues> builder) => builder.Statement;
    }

    public class GroupBy<TValues, TGroupBy>
    {
        private readonly TaggedMeasurement measurement;
        private readonly SelectClause select;
        private readonly FromClause from;
        private readonly WhereClause where;
        private readonly GroupByClause groupBy;

        internal GroupBy(TaggedMeasurement measurement, SelectClause select, FromClause from, LambdaExpression tagSelection, WhereClause where = null, TimeSpan? timeInterval = null)
        {
            this.measurement = measurement ?? throw new ArgumentNullException(nameof(measurement));

            this.select = select;
            this.from = from;
            this.where = where;

            var duration = timeInterval.HasValue ? new DurationLiteral(timeInterval.Value) : null;
            var tags = ParseExpression(tagSelection).ToArray();
            this.groupBy = new GroupByClause(duration, tags);
        }

        public MultiSeriesSelectStatement<TValues, TGroupBy> Statement => new MultiSeriesSelectStatement<TValues, TGroupBy>(select, from, where, groupBy);

        public OrderBy<TValues, TGroupBy> OrderByTimeDesc() => new OrderBy<TValues, TGroupBy>(Statement);

        public Fill<TValues, TGroupBy> Fill(FillType type) => new Fill<TValues, TGroupBy>(Statement, type);

        public Limit<TValues, TGroupBy> Limit(int n) => new Limit<TValues, TGroupBy>(Statement, n);

        public Offset<TValues, TGroupBy> Offset(int n) => new Offset<TValues, TGroupBy>(Statement, n);

        public SLimit<TValues, TGroupBy> SLimit(int n) => new SLimit<TValues, TGroupBy>(Statement, n);

        public SOffset<TValues, TGroupBy> SOffset(int n) => new SOffset<TValues, TGroupBy>(Statement, n);

        public override string ToString() => Statement.Text;

        public static implicit operator MultiSeriesSelectStatement<TValues, TGroupBy>(GroupBy<TValues, TGroupBy> builder) => builder.Statement;

        private IEnumerable<MeasurementTag> ParseExpression(LambdaExpression selectExpression)
        {
            Expression tagsParameter = selectExpression.Parameters[0];

            if (selectExpression.Body == tagsParameter)
            {
                // Identity function so all tags selected.
                return measurement.TagSet;
            }

            switch (selectExpression.Body.NodeType)
            {
                case ExpressionType.New:
                    return ParseProjection((NewExpression)selectExpression.Body, tagsParameter);
                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerable<MeasurementTag> ParseProjection(NewExpression expression, Expression tagsParameter)
        {
            for (var i = 0; i < expression.Arguments.Count; i++)
            {
                var member = expression.Members[i];
                var argument = expression.Arguments[i];

                switch (argument.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        if (((MemberExpression)argument).Expression == tagsParameter)
                        {
                            var tag = measurement.TagSet.Single(x => x.DotNetAlias == ((MemberExpression)argument).Member.Name);

                            yield return new MeasurementTag(tag.InfluxTagName, member.Name);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}