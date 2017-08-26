using System.Linq;
using InfluxDB.InfluxQL.Schema;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests
{
    public class SelectStatementTests
    {
        public sealed class Tags
        {
            public string size { get; }
        }

        public sealed class Fields
        {
            public double weight { get; }

            public string height { get; }
        }

        [Fact]
        public void Selected_entire_field_set()
        {
            var testMeasurement = new Measurement<Fields>("test");

            var select = InfluxQuery.From(testMeasurement).Select(fields => fields).Statement;

            select.Columns.ShouldNotBeEmpty();
            select.ColumnNames().ShouldBe(new[] { nameof(Fields.weight), nameof(Fields.height) });
        }

        [Fact]
        public void Select_specific_field()
        {
            var testMeasurement = new TaggedMeasurement<Fields, Tags>("test");

            var select = InfluxQuery.From(testMeasurement).Select(fields => new { fields.height }).Statement;

            select.Columns.ShouldNotBeEmpty();
            select.ColumnNames().ShouldBe(new[] { nameof(Fields.height) });
        }

        [Fact]
        public void Alias_field()
        {
            var testMeasurement = new TaggedMeasurement<Fields, Tags>("test");

            var select = InfluxQuery.From(testMeasurement).Select(fields => new { HowHigh = fields.height }).Statement;

            select.ColumnNames().ShouldBe(new[] { nameof(Fields.height) });
            select.Columns.Select(x => x.Alias).ShouldBe(new[] { "HowHigh" });

            select.ToString().ShouldBe("SELECT height AS HowHigh FROM test");
        }

        [Fact]
        public void Select_specific_field_and_tag()
        {
            var testMeasurement = new TaggedMeasurement<Fields, Tags>("test");

            var select = InfluxQuery.From(testMeasurement).Select((fields, tags) => new { fields.height, tags.size }).Statement;

            select.Columns.ShouldNotBeEmpty();
            select.ColumnNames().ShouldBe(new[] { nameof(Fields.height), nameof(Tags.size) });
        }
    }
}