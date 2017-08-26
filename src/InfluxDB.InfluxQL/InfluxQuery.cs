using InfluxDB.InfluxQL.Schema;
using InfluxDB.InfluxQL.StatementBuilders.SelectStatement;

namespace InfluxDB.InfluxQL
{
    public static class InfluxQuery
    {
        public static From<TFields> From<TFields>(Measurement<TFields> measurement)
        {
            return new From<TFields>(measurement);
        }

        public static From<TFields, TTags> From<TFields, TTags>(TaggedMeasurement<TFields, TTags> measurement)
        {
            return new From<TFields, TTags>(measurement);
        }
    }
}