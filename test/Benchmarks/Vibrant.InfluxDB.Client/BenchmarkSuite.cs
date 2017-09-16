using System;
using System.Linq;
using System.Threading.Tasks;
using Benchmarks;

// ReSharper disable CheckNamespace
namespace Vibrant.InfluxDB.Client
{
    public class BenchmarkSuite : IBenchmarkSuite
    {
        public async Task<double> SumTheWaterLevel(string endpoint, string database)
        {
            var client = new InfluxClient(new Uri(endpoint));

            var query = "SELECT water_level FROM h2o_feet";

            var resultSet = await client.ReadAsync<WaterDepth>(database, query);

            var series = resultSet.Results.Single().Series.Single();

            double total = 0;

            foreach (var row in series.Rows)
            {
                total += row.WaterLevel;
            }

            return total;
        }

        public class WaterDepth
        {
            [InfluxTimestamp]
            public DateTime Timestamp { get; set; }

            [InfluxTag("location")]
            public string Location { get; set; }

            [InfluxField("level_description")]
            public string LevelDescription { get; set; }

            [InfluxField("water_level")]
            public double WaterLevel { get; set; }
        }
    }
}