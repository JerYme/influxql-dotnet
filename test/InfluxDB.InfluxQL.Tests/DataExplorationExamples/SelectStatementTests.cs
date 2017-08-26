using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests.DataExplorationExamples
{
    public class SelectStatementTests : NoaaSampleDataTest
    {
        [Fact]
        public void Select_all_fields_from_a_single_measurement()
        {
            // The example of this is "SELECT *::field FROM h2o_feet";
            // Instead we know which fields we require so identify them explicitly.

            var query = InfluxQuery.From(h2o_feet).Select(fields => fields);

            query.Statement.Text.ShouldBe("SELECT \"level description\" AS level_description, water_level FROM h2o_feet");
        }

        [Fact]
        public void Select_specific_tags_and_fields_from_a_single_measurement()
        {
            var query = InfluxQuery.From(h2o_feet).Select((fields, tags) => new { fields.level_description, tags.location, fields.water_level });

            query.Statement.Text.ShouldBe("SELECT \"level description\" AS level_description, location, water_level FROM h2o_feet");
        }
    }
}