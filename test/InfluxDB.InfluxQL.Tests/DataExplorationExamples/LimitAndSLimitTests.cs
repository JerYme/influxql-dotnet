using System;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests.DataExplorationExamples
{
    public class LimitAndSLimitTests : NoaaSampleDataTest
    {
        [Fact]
        public void Limit_the_number_of_points_and_series_returned()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select(f => new { f.water_level })
                .GroupBy(t => t)
                .Limit(3)
                .SLimit(1);

            query.Statement.Text.ShouldBe("SELECT water_level FROM h2o_feet GROUP BY location LIMIT 3 SLIMIT 1");
        }

        [Fact]
        public void Limit_the_number_of_points_and_series_returned_and_include_a_GROUP_BY_time_clause()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select(f => new { mean = Aggregations.MEAN(f.water_level) })
                .Where("time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:42:00Z'")
                .GroupBy(TimeSpan.FromMinutes(12), t => t)
                .Limit(2)
                .SLimit(1);

            query.Statement.Text.ShouldBe("SELECT MEAN(water_level) AS mean FROM h2o_feet WHERE time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:42:00Z' GROUP BY time(12m),location LIMIT 2 SLIMIT 1");
        }
    }
}