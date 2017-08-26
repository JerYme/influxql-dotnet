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
        public MultiSeriesSelectStatement(SelectClause select, FromClause from, WhereClause where = null, GroupByClause groupBy = null)
        {
            Select = select ?? throw new ArgumentNullException(nameof(select));
            From = from ?? throw new ArgumentNullException(nameof(from));
            Where = where;
            GroupBy = groupBy;
        }

        public SelectClause Select { get; }

        public FromClause From { get; }

        public WhereClause Where { get; }

        public GroupByClause GroupBy { get; }

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

            return statement.ToString();
        }
    }
}