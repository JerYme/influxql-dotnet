using System;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests.DataExplorationExamples
{
    public class SOffsetClauseTests : NoaaSampleDataTest
    {
        [Fact]
        public void Paginate_series()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select(f => new { f.water_level })
                .GroupBy(t => t)
                .SLimit(1)
                .SOffset(1);

            query.Statement.Text.ShouldBe("SELECT water_level FROM h2o_feet GROUP BY location SLIMIT 1 SOFFSET 1");
        }

        [Fact(Skip = "ORDER BY not implemented")]
        public void Paginate_series_and_include_several_clauses()
        {
            var query = InfluxQuery.From(h2o_feet)
                .Select(f => new { mean = Aggregations.MEAN(f.water_level) })
                .Where("time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:42:00Z'")
                .GroupBy(TimeSpan.FromMinutes(12), t => t)
                .Limit(2)
                .Offset(2)
                .SLimit(1)
                .SOffset(1);

            query.Statement.Text.ShouldBe("SELECT MEAN(water_level) AS mean FROM h2o_feet WHERE time >= '2015-08-18T00:00:00Z' AND time <= '2015-08-18T00:42:00Z' GROUP BY time(12m),location ORDER BY time DESC LIMIT 2 OFFSET 2 SLIMIT 1 SOFFSET 1");
        }
    }
}