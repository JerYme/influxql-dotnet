using InfluxDB.InfluxQL.Syntax.Clauses;
using InfluxDB.InfluxQL.Syntax.Statements;

namespace InfluxDB.InfluxQL.StatementBuilders.SelectStatement
{
    public class OrderBy<TValues>
    {
        internal OrderBy(SingleSeriesSelectStatement<TValues> statement)
        {
            Statement = new SingleSeriesSelectStatement<TValues>(statement.Select, statement.From, statement.Where, statement.GroupBy, new OrderByClause());
        }

        public SingleSeriesSelectStatement<TValues> Statement { get; }

        public Limit<TValues> Limit(int n)
        {
            return new Limit<TValues>(Statement, n);
        }

        public Offset<TValues> Offset(int n)
        {
            return new Offset<TValues>(Statement, n);
        }

        public override string ToString()
        {
            return Statement.Text;
        }

        public static implicit operator SingleSeriesSelectStatement<TValues>(OrderBy<TValues> builder)
        {
            return builder.Statement;
        }
    }

    public class OrderBy<TValues, TGroupBy>
    {
        internal OrderBy(MultiSeriesSelectStatement<TValues, TGroupBy> statement)
        {
            Statement = new MultiSeriesSelectStatement<TValues, TGroupBy>(statement.Select, statement.From, statement.Where, statement.GroupBy, new OrderByClause());
        }

        public MultiSeriesSelectStatement<TValues, TGroupBy> Statement { get; }

        public Limit<TValues, TGroupBy> Limit(int n)
        {
            return new Limit<TValues, TGroupBy>(Statement, n);
        }

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

        public static implicit operator MultiSeriesSelectStatement<TValues, TGroupBy>(OrderBy<TValues, TGroupBy> builder)
        {
            return builder.Statement;
        }
    }
}