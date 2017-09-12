using InfluxDB.InfluxQL.Schema;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests.Schema
{
    public class AnonymousMeasurementDefinitionTests
    {
        [Fact]
        public void Can_define_measurment_using_anonymous_type()
        {
            var temperature = Measurement.Define("h2o_temperature", new { degrees = default(float) });

            temperature.FieldSet.ShouldBe(new[] { new MeasurementField("degrees") });
        }

        [Fact]
        public void Can_define_tagged_measurment_using_anonymous_type()
        {
            var temperature = Measurement.Define("h2o_temperature", new { degrees = default(float) }, new { location = default(string) });

            temperature.FieldSet.ShouldBe(new[] { new MeasurementField("degrees") });
            temperature.TagSet.ShouldBe(new[] { new MeasurementTag("location") });
        }

        [Fact]
        public void Can_alias_fields_on_measurment()
        {
            var h2o_feet = Measurement.Define("h2o_feet", new { water_level = default(float), level_description = default(string) })
                .SetInfluxFieldName(x => x.level_description, "level description");

            h2o_feet.FieldSet.ShouldBe(new[] { new MeasurementField("water_level"), new MeasurementField("level_description", "level description") });
        }

        [Fact]
        public void Can_alias_fields_on_taged_measurment()
        {
            var h2o_feet = Measurement.Define("h2o_feet", new { water_level = default(float), level_description = default(string) }, new { location = default(string) })
                .SetInfluxFieldName(x => x.level_description, "level description");

            h2o_feet.FieldSet.ShouldBe(new[] { new MeasurementField("water_level"), new MeasurementField("level_description", "level description") });
        }

        [Fact]
        public void Can_alias_tags()
        {
            var measurement = Measurement.Define("my_measurement", new { level = default(float) }, new { location = default(string), mood = default(string) })
                .SetInfluxTagName(x => x.mood, "😃 or sad");

            measurement.TagSet.ShouldBe(new[] { new MeasurementTag("location"), new MeasurementTag("mood", "😃 or sad") });
        }
    }
}