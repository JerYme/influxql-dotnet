using System;
using System.Collections.Generic;
using System.Text;
using InfluxDB.InfluxQL.Syntax.Clauses;
using InfluxDB.InfluxQL.Syntax.Expressions;

namespace InfluxDB.InfluxQL.Syntax.Statements
{
    public class SingleSeriesSelectStatement<TValues>
    {
        public SingleSeriesSelectStatement(SelectClause select, FromClause from, WhereClause where = null, GroupByClause groupBy = null)
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

        public IReadOnlyCollection<ColumnSelectionExpression> Columns => Select.Columns;

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