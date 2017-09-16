using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Benchmarks;

// ReSharper disable CheckNamespace
namespace InfluxClient
{
    public class BenchmarkSuite : IBenchmarkSuite
    {
        public async Task<double> SumTheWaterLevel(string endpoint, string database)
        {
            var mgr = new InfluxManager(endpoint, database);

            var response = await mgr.Query("SELECT water_level FROM h2o_feet");

            var serie = response.Results.Single().Series.Single();

            decimal total = 0;
            foreach (var values in serie.Values)
            {
                // value type seems to depend if value has decimal places.
                switch (((object[])values)[1])
                {
                    case decimal water_level:
                        total += water_level;
                        break;
                    case int water_level:
                        total += water_level;
                        break;
                }
            }

            return (double)total;
        }
    }
}