using System;
using System.Threading.Tasks;
using Benchmarks;
using InfluxDB.InfluxQL.Client;
using InfluxDB.InfluxQL.Tests.DataExplorationExamples.NoaaSampleData;

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
    }
}