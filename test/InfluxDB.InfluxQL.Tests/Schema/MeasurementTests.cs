using System.Linq;
using InfluxDB.InfluxQL.Schema;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests.Schema
{
    public class MeasurementTests
    {
        public sealed class Tags
        {
            public string location { get; }

            public string name { get; }
        }

        public sealed class Fields
        {
            public int weight { get; }

            public double height { get; }
        }

        public sealed class AliasedFields
        {
            [InfluxKeyName("white")]
            public double black { get; }
        }

        public sealed class AliasedTags
        {
            [InfluxKeyName("smiling face with open mouth \U0001F603")]
            public string happy { get; }
        }

        [Fact]
        public void Measurement_has_name()
        {
            var myMeasuremnt = new TaggedMeasurement<Fields, Tags>("my_measument");

            myMeasuremnt.Name.ShouldBe("my_measument");
        }

        [Fact]
        public void Measurement_has_fields()
        {
            var myMeasuremnt = new Measurement<Fields>("my_measument");

            myMeasuremnt.FieldSet.ShouldNotBeEmpty();
            myMeasuremnt.FieldSet.Select(x => x.InfluxFieldName).ShouldBe(new[] { nameof(Fields.weight), nameof(Fields.height) });
        }

        [Fact]
        public void Measurement_can_have_tags()
        {
            var myMeasuremnt = new TaggedMeasurement<Fields, Tags>("my_measument");

            myMeasuremnt.TagSet.ShouldNotBeEmpty();
            myMeasuremnt.TagSet.Select(x => x.InfluxTagName).ShouldBe(new[] { nameof(Tags.location), nameof(Tags.name) });
        }

        [Fact]
        public void Fields_can_be_aliased_using_attribute()
        {
            var myMeasuremnt = new Measurement<AliasedFields>("my_measument");

            var blackField = myMeasuremnt.FieldSet.Single();

            // The measuremnt field name is what the field is called in influx
            blackField.InfluxFieldName.ShouldBe("white");

            // The alias is the name we use to refer to it (valid C# name).
            blackField.DotNetAlias.ShouldBe(nameof(AliasedFields.black));
        }

        [Fact]
        public void Tags_can_be_aliased_using_attribute()
        {
            var myMeasuremnt = new TaggedMeasurement<Fields, AliasedTags>("my_measument");

            var tag = myMeasuremnt.TagSet.Single();

            // Unicode in influxdb with an emoji
            tag.InfluxTagName.ShouldBe("smiling face with open mouth 😃");

            // But we know it as good
            tag.DotNetAlias.ShouldBe(nameof(AliasedTags.happy));
        }
    }
}