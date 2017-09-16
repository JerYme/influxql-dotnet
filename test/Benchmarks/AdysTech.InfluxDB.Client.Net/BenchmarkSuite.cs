using System.Linq;
using System.Threading.Tasks;
using Benchmarks;

// ReSharper disable CheckNamespace
namespace AdysTech.InfluxDB.Client.Net
{
    public class BenchmarkSuite : IBenchmarkSuite
    {
        public async Task<double> SumTheWaterLevel(string endpoint, string database)
        {
            var client = new InfluxDBClient(endpoint);

            var query = "SELECT water_level FROM h2o_feet";

            var results = await client.QueryMultiSeriesAsync(database, query);

            var serie = results.Single();

            double total = 0;
            foreach (dynamic entry in serie.Entries)
            {
                // Not sure why but water_level comes back as a string.
                total += double.Parse(entry.Water_level);
            }

            return total;
        }
    }
}