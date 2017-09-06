using System;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests.DataExplorationExamples
{
    public class OrderByTimeDescTests : NoaaSampleDataTest
    {
        [Fact]
        public void Return_the_newest_points_first()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select(f => new { f.water_level })
                .Where("location = 'santa_monica'")
                .OrderByTimeDesc();

            query.Statement.Text.ShouldBe("SELECT water_level FROM h2o_feet WHERE location = 'santa_monica' ORDER BY time DESC");
        }

        [Fact]
        public void Return_the_newest_points_first_and_include_a_GROUP_BY_time_clause()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select(f => new { mean = Aggregations.MEAN(f.water_level) })
                .Where("time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:42:00Z'")
                .GroupBy(TimeSpan.FromMinutes(12))
                .OrderByTimeDesc();

            query.Statement.Text.ShouldBe("SELECT MEAN(water_level) AS mean FROM h2o_feet WHERE time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:42:00Z' GROUP BY time(12m) ORDER BY time DESC");
        }
    }
}