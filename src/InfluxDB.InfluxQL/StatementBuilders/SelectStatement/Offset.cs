using InfluxDB.InfluxQL.Syntax.Clauses;
using InfluxDB.InfluxQL.Syntax.Statements;

namespace InfluxDB.InfluxQL.StatementBuilders.SelectStatement
{
    public class Offset<TValues>
    {
        internal Offset(SingleSeriesSelectStatement<TValues> statement, int n)
        {
            Statement = new SingleSeriesSelectStatement<TValues>(statement.Select, statement.From, statement.Where, statement.GroupBy,statement.Fill, statement.OrderBy, statement.Limit, new OffsetClause(n));
        }

        public SingleSeriesSelectStatement<TValues> Statement { get; }

        public override string ToString()
        {
            return Statement.Text;
        }

        public static implicit operator SingleSeriesSelectStatement<TValues>(Offset<TValues> builder)
        {
            return builder.Statement;
        }
    }

    public class Offset<TValues, TGroupBy>
    {
        internal Offset(MultiSeriesSelectStatement<TValues, TGroupBy> statement, int n)
        {
            Statement = new MultiSeriesSelectStatement<TValues, TGroupBy>(statement.Select, statement.From, statement.Where, statement.GroupBy,statement.Fill, statement.OrderBy, statement.Limit, new OffsetClause(n));
        }

        public MultiSeriesSelectStatement<TValues, TGroupBy> Statement { get; }

        public SLimit<TValues, TGroupBy> SLimit(int n)
        {
            return new SLimit<TValues, TGroupBy>(Statement, n);
        }

        public SOffset<TValues, TGroupBy> SOffset(int n)
        {
            return new SOffset<TValues, TGroupBy>(Statement, n);
        }

        public override string ToString()
        {
            return Statement.Text;
        }

        public static implicit operator MultiSeriesSelectStatement<TValues, TGroupBy>(Offset<TValues, TGroupBy> builder)
        {
            return builder.Statement;
        }
    }
}