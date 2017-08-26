using System;
using System.Linq.Expressions;
using InfluxDB.InfluxQL.Schema;
using InfluxDB.InfluxQL.Syntax.Clauses;

namespace InfluxDB.InfluxQL.StatementBuilders.SelectStatement
{
    public class From<TFields>
    {
        private readonly Measurement measurement;
        private readonly FromClause from;

        internal From(Measurement measurement)
        {
            this.measurement = measurement ?? throw new ArgumentNullException(nameof(measurement));

            this.from = new FromClause(measurement);
        }

        public Select<TValues, TFields> Select<TValues>(Expression<Func<TFields, TValues>> select)
        {
            return new Select<TValues, TFields>(measurement, from, select);
        }

        public override string ToString()
        {
            return from.ToString();
        }
    }

    public class From<TFields, TTags>
    {
        private readonly TaggedMeasurement measurement;
        private readonly FromClause from;

        internal From(TaggedMeasurement measurement)
        {
            this.measurement = measurement ?? throw new ArgumentNullException(nameof(measurement));

            this.from = new FromClause(measurement);
        }

        public Select<TValues, TFields, TTags> Select<TValues>(Expression<Func<TFields, TValues>> columnSelection)
        {
            return new Select<TValues, TFields, TTags>(measurement, from, columnSelection);
        }

        public Select<TValues, TFields, TTags> Select<TValues>(Expression<Func<TFields, TTags, TValues>> columnSelection)
        {
            return new Select<TValues, TFields, TTags>(measurement, from, columnSelection);
        }

        public override string ToString()
        {
            return from.ToString();
        }
    }
}