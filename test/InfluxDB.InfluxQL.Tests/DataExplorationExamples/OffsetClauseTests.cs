using System;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests.DataExplorationExamples
{
    public class OffsetClauseTests : NoaaSampleDataTest
    {
        [Fact]
        public void Paginate_points()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select((f, t) => new { f.water_level, t.location })
                .Limit(3)
                .Offset(3);

            query.Statement.Text.ShouldBe("SELECT water_level, location FROM h2o_feet LIMIT 3 OFFSET 3");
        }

        [Fact(Skip = "ORDER BY not implemented")]
        public void Paginate_points_and_include_several_clauses()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select(f => new { mean = Aggregations.MEAN(f.water_level) })
                .Where("time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:42:00Z'")
                .GroupBy(TimeSpan.FromMinutes(12), t => t)
                .Limit(2)
                .Offset(2)
                .SLimit(1);

            query.Statement.Text.ShouldBe("SELECT MEAN(water_level) AS mean FROM h2o_feet WHERE time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:42:00Z' GROUP BY time(12m),location ORDER BY time DESC LIMIT 2 OFFSET 2 SLIMIT 1");
        }
    }
}