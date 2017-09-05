using System;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests.DataExplorationExamples
{
    public class LimitClauseTests : NoaaSampleDataTest
    {
        [Fact]
        public void Limit_the_number_of_points_returned()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select((f, t) => new { f.water_level, t.location })
                .Limit(3);

            query.Statement.Text.ShouldBe("SELECT water_level, location FROM h2o_feet LIMIT 3");
        }

        [Fact]
        public void Limit_the_number_points_returned_and_include_a_GROUP_BY_clause()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select((f, t) => new { mean = Aggregations.MEAN(f.water_level) })
                .Where("time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:42:00Z'")
                .GroupBy(TimeSpan.FromMinutes(12), t => t)
                .Limit(2);

            query.Statement.Text.ShouldBe("SELECT MEAN(water_level) AS mean FROM h2o_feet WHERE time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:42:00Z' GROUP BY time(12m),location LIMIT 2");
        }
    }
}