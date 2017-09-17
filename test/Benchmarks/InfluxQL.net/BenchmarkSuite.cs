using System;
using System.Threading.Tasks;
using Benchmarks;
using InfluxDB.InfluxQL.Client;
using InfluxDB.InfluxQL.Schema;

// ReSharper disable CheckNamespace
namespace InfluxDB.InfluxQL
{
    public class BenchmarkSuite: IBenchmarkSuite
    {
        public async Task<double> SumTheWaterLevel(string endpoint, string database)
        {
            var h2o_feet = new WaterDepth();

            var query = InfluxQuery.From(h2o_feet).Select(x=> new { x.water_level });

            var client = new InfluxQLClient(new Uri(endpoint), database);

            var results = await client.Query(query.Statement);

            double total = 0;
            foreach (var (time, values) in results)
            {
                total += values.water_level;
            }

            return total;
        }

        public sealed class WaterDepth : TaggedMeasurement<WaterDepth.Fields, WaterDepth.Tags>
        {
            public WaterDepth() : base("h2o_feet")
            {
            }

            public struct Tags
            {
                public Tags(string location)
                {
                    this.location = location;
                }

                public string location { get; }
            }

            public struct Fields
            {
                public Fields(string levelDescription, double waterLevel)
                {
                    level_description = levelDescription;
                    water_level = waterLevel;
                }

                [InfluxKeyName("level description")]
                public string level_description { get; }

                public double water_level { get; }
            }
        }
    }
}