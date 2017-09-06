using System;
using System.Linq.Expressions;
using InfluxDB.InfluxQL.Schema;
using InfluxDB.InfluxQL.Syntax.Clauses;
using InfluxDB.InfluxQL.Syntax.Statements;

namespace InfluxDB.InfluxQL.StatementBuilders.SelectStatement
{
    public class Where<TValues>
    {
        private readonly SelectClause select;
        private readonly FromClause from;
        private readonly WhereClause where;

        public Where(SelectClause select, FromClause from, string predicate)
        {
            this.select = select;
            this.from = from;
            this.where = new WhereClause(predicate);
        }

        public SingleSeriesSelectStatement<TValues> Statement => new SingleSeriesSelectStatement<TValues>(select, from, where);

        public GroupBy<TValues> GroupBy(TimeSpan timeInterval)
        {
            return new GroupBy<TValues>(select, from, timeInterval, where);
        }

        public OrderBy<TValues> OrderByTimeDesc()
        {
            return new OrderBy<TValues>(Statement);
        }

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
            return Statement.ToString();
        }

        public static implicit operator SingleSeriesSelectStatement<TValues>(Where<TValues> builder)
        {
            return builder.Statement;
        }
    }

    public class Where<TValues, TTags>
    {
        private readonly TaggedMeasurement measurement;
        private readonly SelectClause select;
        private readonly FromClause from;
        private readonly WhereClause where;

        internal Where(TaggedMeasurement measurement, SelectClause select, FromClause from, string predicate)
        {
            this.measurement = measurement ?? throw new ArgumentNullException(nameof(measurement));
            this.select = select ?? throw new ArgumentNullException(nameof(select));
            this.from = from ?? throw new ArgumentNullException(nameof(from));
            this.where = new WhereClause(predicate);
        }

        public SingleSeriesSelectStatement<TValues> Statement => new SingleSeriesSelectStatement<TValues>(select, from, where);

        public GroupBy<TValues> GroupBy(TimeSpan timeInterval)
        {
            return new GroupBy<TValues>(select, from, timeInterval, where);
        }

        public GroupBy<TValues, TGroupBy> GroupBy<TGroupBy>(Expression<Func<TTags, TGroupBy>> tagSelection)
        {
            return new GroupBy<TValues, TGroupBy>(measurement, select, from, tagSelection, where);
        }

        public GroupBy<TValues, TGroupBy> GroupBy<TGroupBy>(TimeSpan timeInterval, Expression<Func<TTags, TGroupBy>> tagSelection)
        {
            return new GroupBy<TValues, TGroupBy>(measurement, select, from, tagSelection, where, timeInterval);
        }

        public OrderBy<TValues> OrderByTimeDesc()
        {
            return new OrderBy<TValues>(Statement);
        }

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
            return Statement.ToString();
        }

        public static implicit operator SingleSeriesSelectStatement<TValues>(Where<TValues, TTags> builder)
        {
            return builder.Statement;
        }
    }
}