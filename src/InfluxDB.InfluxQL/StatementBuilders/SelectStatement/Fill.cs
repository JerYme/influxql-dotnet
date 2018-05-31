using InfluxDB.InfluxQL.Syntax.Clauses;
using InfluxDB.InfluxQL.Syntax.Statements;

namespace InfluxDB.InfluxQL.StatementBuilders.SelectStatement
{
    public class Fill<TValues>
    {
        internal Fill(SingleSeriesSelectStatement<TValues> statement, FillType type)
        {
            Statement = new SingleSeriesSelectStatement<TValues>(statement.Select, statement.From, statement.Where, statement.GroupBy, new FillClause(type));
        }

        public SingleSeriesSelectStatement<TValues> Statement { get; }

        public Limit<TValues> Limit(int n) => new Limit<TValues>(Statement, n);

        public Offset<TValues> Offset(int n) => new Offset<TValues>(Statement, n);

        public override string ToString() => Statement.Text;

        public static implicit operator SingleSeriesSelectStatement<TValues>(Fill<TValues> builder) => builder.Statement;
    }

    public class Fill<TValues, TGroupBy>
    {
        internal Fill(MultiSeriesSelectStatement<TValues, TGroupBy> statement, FillType type)
        {
            Statement = new MultiSeriesSelectStatement<TValues, TGroupBy>(statement.Select, statement.From, statement.Where, statement.GroupBy, new FillClause(type));
        }

        public MultiSeriesSelectStatement<TValues, TGroupBy> Statement { get; }

        public Limit<TValues, TGroupBy> Limit(int n) => new Limit<TValues, TGroupBy>(Statement, n);

        public Offset<TValues, TGroupBy> Offset(int n) => new Offset<TValues, TGroupBy>(Statement, n);

        public SLimit<TValues, TGroupBy> SLimit(int n) => new SLimit<TValues, TGroupBy>(Statement, n);

        public SOffset<TValues, TGroupBy> SOffset(int n) => new SOffset<TValues, TGroupBy>(Statement, n);

        public override string ToString() => Statement.Text;

        public static implicit operator MultiSeriesSelectStatement<TValues, TGroupBy>(Fill<TValues, TGroupBy> builder) => builder.Statement;
    }

}