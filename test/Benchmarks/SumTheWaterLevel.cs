using System;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class SumTheWaterLevel
    {
        private readonly string endpoint = "http://localhost:8086";
        private readonly string database = "NOAA_water_database";

        [Benchmark(Baseline = true)]
        public async Task<double> JustMakeTheCall()
        {
            var query = "SELECT water_level FROM h2o_feet";

            var httpClient = new HttpClient { BaseAddress = new Uri(endpoint) };

            var response = await httpClient.GetAsync($"query?db={Uri.EscapeDataString(database)}&q={Uri.EscapeDataString(query)}");
            response.EnsureSuccessStatusCode();

            await response.Content.ReadAsByteArrayAsync();

            return 0;
        }

        [Benchmark]
        public async Task<double> MakeTheCallWithEpoch()
        {
            var query = "SELECT water_level FROM h2o_feet";

            var httpClient = new HttpClient { BaseAddress = new Uri(endpoint) };

            var response = await httpClient.GetAsync($"query?db={Uri.EscapeDataString(database)}&q={Uri.EscapeDataString(query)}&epoch=ns");
            response.EnsureSuccessStatusCode();

            await response.Content.ReadAsByteArrayAsync();

            return 0;
        }

        [Benchmark(Description = "InfluxQL.net by gambrose")]
        public Task<double> by_gambrose()
        {
            return new InfluxDB.InfluxQL.BenchmarkSuite().SumTheWaterLevel(endpoint, database);
        }

        [Benchmark(Description = "Alt InfluxQL.net by gambrose")]
        public Task<double> alt_by_gambrose()
        {
            return new InfluxDB.InfluxQL.BenchmarkSuite().SumTheWaterLevelAlt(endpoint, database);
        }

        //[Benchmark(Description = "InfluxData.Net by pootzko")]
        //public Task<double> by_pootzko()
        //{
        //    return new InfluxData.Net.InfluxDb.BenchmarkSuite().SumTheWaterLevel(endpoint, database);
        //}

        //[Benchmark(Description = "InfluxDB Client for .NET by MikaelGRA")]
        //public Task<double> by_MikaelGRA()
        //{
        //    return new Vibrant.InfluxDB.Client.BenchmarkSuite().SumTheWaterLevel(endpoint, database);
        //}

        //[Benchmark(Description = "InfluxDB.Client.Net by mvadu")]
        //public Task<double> by_mvadu()
        //{
        //    return new AdysTech.InfluxDB.Client.Net.BenchmarkSuite().SumTheWaterLevel(endpoint, database);
        //}

        //[Benchmark(Description = "InfluxClient by danesparza")]
        //public Task<double> by_danesparza()
        //{
        //    return new InfluxClient.BenchmarkSuite().SumTheWaterLevel(endpoint, database);
        //}
    }
}