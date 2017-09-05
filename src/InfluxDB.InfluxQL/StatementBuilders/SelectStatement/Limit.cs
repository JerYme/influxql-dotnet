using InfluxDB.InfluxQL.Syntax.Clauses;
using InfluxDB.InfluxQL.Syntax.Statements;

namespace InfluxDB.InfluxQL.StatementBuilders.SelectStatement
{
    public class Limit<TValues>
    {
        internal Limit(SingleSeriesSelectStatement<TValues> statement, int n)
        {
            Statement = new SingleSeriesSelectStatement<TValues>(statement.Select, statement.From, statement.Where, statement.GroupBy, new LimitClause(n));
        }

        public SingleSeriesSelectStatement<TValues> Statement { get; }

        public Offset<TValues> Offset(int n)
        {
            return new Offset<TValues>(Statement, n);
        }

        public override string ToString()
        {
            return Statement.Text;
        }

        public static implicit operator SingleSeriesSelectStatement<TValues>(Limit<TValues> builder)
        {
            return builder.Statement;
        }
    }

    public class Limit<TValues, TGroupBy>
    {
        internal Limit(MultiSeriesSelectStatement<TValues, TGroupBy> statement, int n)
        {
            Statement = new MultiSeriesSelectStatement<TValues, TGroupBy>(statement.Select, statement.From, statement.Where, statement.GroupBy, new LimitClause(n));
        }

        public MultiSeriesSelectStatement<TValues, TGroupBy> Statement { get; }

        public Offset<TValues, TGroupBy> Offset(int n)
        {
            return new Offset<TValues, TGroupBy>(Statement, n);
        }

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

        public static implicit operator MultiSeriesSelectStatement<TValues, TGroupBy>(Limit<TValues, TGroupBy> builder)
        {
            return builder.Statement;
        }
    }
}