using System;
using System.Collections.Immutable;
using System.Text;
using InfluxDB.InfluxQL.Schema;
using InfluxDB.InfluxQL.Syntax.Clauses;
using InfluxDB.InfluxQL.Syntax.Expressions;

namespace InfluxDB.InfluxQL.Syntax.Statements
{
    public class MultiSeriesSelectStatement<TValues, TTags>
    {
        public MultiSeriesSelectStatement(
            SelectClause select,
            FromClause from,
            WhereClause where = null,
            GroupByClause groupBy = null,
            FillClause fill = null,
            OrderByClause orderBy = null,
            LimitClause limit = null,
            OffsetClause offset = null,
            SLimitClause slimit = null,
            SOffsetClause soffset = null
        )
        {
            Select = select ?? throw new ArgumentNullException(nameof(select));
            From = from ?? throw new ArgumentNullException(nameof(from));
            Where = where;
            GroupBy = groupBy;
            Fill = fill;
            OrderBy = orderBy;
            Limit = limit;
            Offset = offset;
            SLimit = slimit;
            SOffset = soffset;
        }

        public SelectClause Select { get; }

        public FromClause From { get; }

        public WhereClause Where { get; }

        public GroupByClause GroupBy { get; }

        public FillClause Fill { get; }

        public OrderByClause OrderBy { get; }

        public LimitClause Limit { get; }

        public OffsetClause Offset { get; }

        public SLimitClause SLimit { get; }

        public SOffsetClause SOffset { get; }

        public string Text => this.ToString();

        public ImmutableList<ColumnSelectionExpression> Columns => Select.Columns;

        public ImmutableList<MeasurementTag> Tags => GroupBy.Tags;

        public override string ToString()
        {
            var statement = new StringBuilder();

            statement.Append(Select).Append(" ").Append(From);

            if (Where != null)
            {
                statement.Append(" ").Append(Where);
            }

            if (GroupBy != null)
            {
                statement.Append(" ").Append(GroupBy);
            }

            if (Fill != null)
            {
                statement.Append(" ").Append(Fill);
            }

            if (OrderBy != null)
            {
                statement.Append(" ").Append(OrderBy);
            }

            if (Limit != null)
            {
                statement.Append(" ").Append(Limit);
            }

            if (Offset != null)
            {
                statement.Append(" ").Append(Offset);
            }

            if (SLimit != null)
            {
                statement.Append(" ").Append(SLimit);
            }

            if (SOffset != null)
            {
                statement.Append(" ").Append(SOffset);
            }

            return statement.ToString();
        }
    }
}