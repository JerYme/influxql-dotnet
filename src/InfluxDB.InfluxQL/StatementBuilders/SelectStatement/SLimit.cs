using InfluxDB.InfluxQL.Syntax.Clauses;
using InfluxDB.InfluxQL.Syntax.Statements;

namespace InfluxDB.InfluxQL.StatementBuilders.SelectStatement
{
    public class SLimit<TValues, TGroupBy>
    {
        internal SLimit(MultiSeriesSelectStatement<TValues, TGroupBy> statement, int n)
        {
            Statement = new MultiSeriesSelectStatement<TValues, TGroupBy>(
                statement.Select,
                statement.From,
                statement.Where,
                statement.GroupBy,
                statement.Fill,
                statement.OrderBy,
                statement.Limit,
                statement.Offset,
                new SLimitClause(n)
            );
        }

        public MultiSeriesSelectStatement<TValues, TGroupBy> Statement { get; }

        public SOffset<TValues, TGroupBy> SOffset(int n)
        {
            return new SOffset<TValues, TGroupBy>(Statement, n);
        }

        public override string ToString()
        {
            return Statement.Text;
        }

        public static implicit operator MultiSeriesSelectStatement<TValues, TGroupBy>(SLimit<TValues, TGroupBy> builder)
        {
            return builder.Statement;
        }
    }
}