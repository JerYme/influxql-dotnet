using System.Collections.Generic;
using System.Collections.Immutable;
using InfluxDB.InfluxQL.Syntax.Expressions;

namespace InfluxDB.InfluxQL.Syntax.Clauses
{
    public class SelectClause
    {
        internal SelectClause(IEnumerable<ColumnSelectionExpression> columns)
        {
            this.Columns = columns.ToImmutableList();
        }

        public ImmutableList<ColumnSelectionExpression> Columns { get; }

        public override string ToString()
        {
            return $"SELECT {string.Join(", ", Columns)}";
        }
    }
}