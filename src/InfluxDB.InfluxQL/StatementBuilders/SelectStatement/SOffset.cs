using InfluxDB.InfluxQL.Syntax.Clauses;
using InfluxDB.InfluxQL.Syntax.Statements;

namespace InfluxDB.InfluxQL.StatementBuilders.SelectStatement
{
    public class SOffset<TValues, TGroupBy>
    {
        internal SOffset(MultiSeriesSelectStatement<TValues, TGroupBy> statement, int n)
        {
            Statement = new MultiSeriesSelectStatement<TValues, TGroupBy>(
                statement.Select,
                statement.From,
                statement.Where,
                statement.GroupBy,
                statement.OrderBy,
                statement.Limit,
                statement.Offset,
                statement.SLimit,
                new SOffsetClause(n)
            );
        }

        public MultiSeriesSelectStatement<TValues, TGroupBy> Statement { get; }

        public override string ToString()
        {
            return Statement.Text;
        }

        public static implicit operator MultiSeriesSelectStatement<TValues, TGroupBy>(SOffset<TValues, TGroupBy> builder)
        {
            return builder.Statement;
        }
    }
}