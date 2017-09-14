using System;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.InfluxQL.Client;
using InfluxDB.InfluxQL.Tests.DataExplorationExamples.NoaaSampleData;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests.Client
{
    [Trait("Category", "Integration")]
    public class QueryTests : IClassFixture<NoaaWaterDatabaseFixture>
    {
        private readonly NoaaWaterDatabaseFixture fixture;

        public QueryTests(NoaaWaterDatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task Single_series_field_projection_to_anonymous_type()
        {
            var h2o_feet = new WaterDepth();

            var query = InfluxQuery.From(h2o_feet)
                .Select(fields => new { fields.water_level })
                .Where("location = 'santa_monica'");

            var results = await fixture.Client.Query(query.Statement);

            var expectedFirstTwoPoints = new[]
            {
                    (new DateTime(2015, 8, 18, 0, 0, 0, DateTimeKind.Utc), new {water_level = 2.064} ),
                    (new DateTime(2015, 8, 18, 0, 6, 0, DateTimeKind.Utc), new {water_level = 2.116} )
                };

            results.Take(2).ToArray().ShouldBe(expectedFirstTwoPoints);

            var expectedLastTwoPoints = new[]
            {
                    (new DateTime(2015, 9, 18, 21, 36, 0, DateTimeKind.Utc), new {water_level = 5.066} ),
                    ( new DateTime(2015, 9, 18, 21, 42, 0, DateTimeKind.Utc), new {water_level = 4.938} )
                };

            results.Reverse().Take(2).Reverse().ToArray().ShouldBe(expectedLastTwoPoints);
        }

        [Fact]
        public async Task Single_series_all_fields_as_immutable_struct()
        {
            var h2o_feet = new WaterDepth();

            var query = InfluxQuery.From(h2o_feet)
                .Select(fields => fields)
                .Where("location = 'santa_monica'");

            var results = await fixture.Client.Query(query.Statement);

            foreach (var (time, values) in results.Take(1))
            {
                values.level_description.ShouldBe("below 3 feet");
                values.water_level.ShouldBe(2.064);
                time.ShouldBe(new DateTime(2015, 8, 18, 0, 0, 0, DateTimeKind.Utc));
            }
        }

        [Fact]
        public async Task Single_series_field_and_tag_projection()
        {
            var h2o_feet = new WaterDepth();

            var query = InfluxQuery.From(h2o_feet)
                .Select((fields, tags) => new { fields.level_description, tags.location, fields.water_level })
                .Where("location = 'santa_monica'");

            var results = await fixture.Client.Query(query.Statement);

            foreach (var (time, values) in results.Take(1))
            {
                values.level_description.ShouldBe("below 3 feet");
                values.water_level.ShouldBe(2.064);
                values.location.ShouldBe("santa_monica");
                time.ShouldBe(new DateTime(2015, 8, 18, 0, 0, 0, DateTimeKind.Utc));
            }
        }

        [Fact]
        public async Task Single_series_all_fields_grouped_by_all_tags()
        {
            var h2o_feet = new WaterDepth();

            var query = InfluxQuery.From(h2o_feet)
                .Select(fields => fields)
                .Where("location = 'santa_monica'")
                .GroupBy(tags => tags);

            var results = await fixture.Client.Query(query.Statement);

            foreach (var (time, values, tags) in results.Flatten().Take(1))
            {
                values.level_description.ShouldBe("below 3 feet");
                values.water_level.ShouldBe(2.064);
                tags.location.ShouldBe("santa_monica");
                time.ShouldBe(new DateTime(2015, 8, 18, 0, 0, 0, DateTimeKind.Utc));
            }
        }
    }
}