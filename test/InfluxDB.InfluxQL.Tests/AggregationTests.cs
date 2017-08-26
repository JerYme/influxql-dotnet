using InfluxDB.InfluxQL.Syntax;
using System.Linq;
using InfluxDB.InfluxQL.Schema;
using InfluxDB.InfluxQL.Syntax.Expressions;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests
{
    public class AggregationTests
    {
        public sealed class Fields
        {
            public double value { get; }
        }

        [Fact]
        public void Count()
        {
            var testMeasurement = new Measurement<Fields>("test");

            var select = InfluxQuery.From(testMeasurement).Select(fields => new { count = Aggregations.COUNT(fields.value) }).Statement;

            var column = select.Columns.Single() as FieldSelectionExpression;

            column.FieldName.ShouldBe(nameof(Fields.value));
            column.Aggregation.Name.ShouldBe("COUNT");

            select.ToString().ShouldBe("SELECT COUNT(value) AS count FROM test");
        }
    }
}