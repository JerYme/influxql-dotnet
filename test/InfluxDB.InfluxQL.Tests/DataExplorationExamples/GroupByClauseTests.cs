using System;
using Xunit;
using Shouldly;
using static InfluxDB.InfluxQL.Aggregations;

namespace InfluxDB.InfluxQL.Tests.DataExplorationExamples
{
    public class GroupByClauseTests : NoaaSampleDataTest
    {
        [Fact]
        public void Select_all_fields_and_tags_from_a_single_measurement()
        {
            // The example of this is "SELECT * FROM h2o_feet";
            // This would project tags and fields into one obejct. We cannot support this in C# without createing a new type.
            // Instead query using group by (which is more effiecten on the wire) then flatten in the results when we itterate.

            var query = InfluxQuery.From(h2o_feet)
                .Select(fields => fields)
                .GroupBy(tags => tags);

            query.Statement.Text.ShouldBe("SELECT \"level description\" AS level_description, water_level FROM h2o_feet GROUP BY location");
        }

        [Fact]
        public void Group_query_results_by_a_single_tag()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select(fields => new { mean = MEAN(fields.water_level) })
                .GroupBy(tags => new { tags.location });

            query.Statement.Text.ShouldBe("SELECT MEAN(water_level) AS mean FROM h2o_feet GROUP BY location");
        }

        [Fact]
        public void Group_query_results_into_12_minute_intervals()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select(fields => new { count = COUNT(fields.water_level) })
                .Where("location='coyote_creek' AND time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:30:00Z'")
                .GroupBy(TimeSpan.FromMinutes(12));

            query.Statement.Text.ShouldBe("SELECT COUNT(water_level) AS count FROM h2o_feet WHERE location='coyote_creek' AND time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:30:00Z' GROUP BY time(12m)");
        }

        [Fact]
        public void Group_query_results_into_12_minutes_intervals_and_by_a_tag_key()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select(fields => new { count = COUNT(fields.water_level) })
                .Where("time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:30:00Z'")
                .GroupBy(TimeSpan.FromMinutes(12), tags => new { tags.location });

            query.Statement.Text.ShouldBe("SELECT COUNT(water_level) AS count FROM h2o_feet WHERE time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:30:00Z' GROUP BY time(12m),location");
        }
    }
}