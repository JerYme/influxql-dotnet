using System;
using InfluxDB.InfluxQL.Schema;

namespace InfluxDB.InfluxQL.Syntax.Clauses
{
    public class FromClause
    {
        private readonly Measurement measurement;

        internal FromClause(Measurement measurement)
        {
            this.measurement = measurement ?? throw new ArgumentNullException(nameof(measurement));
        }

        public override string ToString()
        {
            return $"FROM {measurement.Name}";
        }
    }
}