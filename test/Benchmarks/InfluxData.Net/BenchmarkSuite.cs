using System;
using System.Linq;
using System.Threading.Tasks;
using Benchmarks;
using InfluxData.Net.Common.Enums;

// ReSharper disable CheckNamespace
namespace InfluxData.Net.InfluxDb
{
    public class BenchmarkSuite: IBenchmarkSuite
    {
        public async Task<double> SumTheWaterLevel(string endpoint, string database)
        {
            var influxDbClient = new InfluxDbClient(endpoint, string.Empty, string.Empty, InfluxDbVersion.Latest);

            var query = "SELECT water_level FROM h2o_feet";

            var response = await influxDbClient.Client.QueryAsync(query, database);

            var serie = response.Single();

            double total = 0;
            foreach (var values in serie.Values)
            {
                // value type seems to depend if value has decimal places.
                switch (values[1])
                {
                    case double water_level:
                        total += water_level;
                        break;
                    case long water_level:
                        total += water_level;
                        break;
                }
            }

            return total;
        }
    }
}