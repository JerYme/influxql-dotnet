using Xunit;
using Shouldly;

namespace InfluxDB.InfluxQL.Tests.DataExplorationExamples
{
    public class WhereClauseTests : NoaaSampleDataTest
    {
        [Fact]
        public void Select_data_that_have_specific_field_key_value()
        {
            // SELECT * FROM "h2o_feet" WHERE "water_level" > 8

            var query = InfluxQuery.From(h2o_feet)
                .Select((fields, tags) => new { fields.water_level, fields.level_description, tags.location })
                .Where("water_level > 8");

            query.Statement.Text.ShouldBe("SELECT water_level, \"level description\" AS level_description, location FROM h2o_feet WHERE water_level > 8");
        }

        [Fact]
        public void Select_data_that_have_a_specific_string_field_key_value()
        {
            // SELECT * FROM "h2o_feet" WHERE "level description" = 'below 3 feet'

            var query = InfluxQuery.From(h2o_feet)
                .Select((fields, tags) => new { fields.water_level, fields.level_description, tags.location })
                .Where("\"level description\" = 'below 3 feet'");

            query.Statement.Text.ShouldBe("SELECT water_level, \"level description\" AS level_description, location FROM h2o_feet WHERE \"level description\" = 'below 3 feet'");
        }

        [Fact]
        public void Select_data_that_have_a_specific_field_key_value_and_perform_basic_arithmetic()
        {
            // SELECT * FROM "h2o_feet" WHERE "water_level" + 2 > 11.9

            var query = InfluxQuery.From(h2o_feet)
                .Select((fields, tags) => new { fields.water_level, fields.level_description, tags.location })
                .Where("water_level + 2 > 11.9");

            query.Statement.Text.ShouldBe("SELECT water_level, \"level description\" AS level_description, location FROM h2o_feet WHERE water_level + 2 > 11.9");
        }

        [Fact]
        public void Select_data_that_have_a_specific_tag_key_value()
        {
            // SELECT "water_level" FROM "h2o_feet" WHERE "location" = 'santa_monica'

            var query = InfluxQuery.From(h2o_feet)
                .Select(fields => new { fields.water_level })
                .Where("location = 'santa_monica'");

            query.Statement.Text.ShouldBe("SELECT water_level FROM h2o_feet WHERE location = 'santa_monica'");
        }

        [Fact]
        public void Select_data_that_have_a_specific_field_key_values_and_tag_key_values()
        {
            // SELECT "water_level" FROM "h2o_feet" WHERE "location" <> 'santa_monica' AND (water_level < -0.59 OR water_level > 9.95)
        }

        [Fact]
        public void Select_data_that_have_specific_timestamps()
        {
            // SELECT * FROM "h2o_feet" WHERE time > now() - 7d
        }
    }
}