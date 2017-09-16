using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Benchmarks;
using InfluxDB.InfluxQL.Client;
using InfluxDB.InfluxQL.Syntax.Statements;
using InfluxDB.InfluxQL.Tests.DataExplorationExamples.NoaaSampleData;
using Newtonsoft.Json;

// ReSharper disable CheckNamespace
namespace InfluxDB.InfluxQL
{
    public class BenchmarkSuite : IBenchmarkSuite
    {
        public async Task<double> SumTheWaterLevel(string endpoint, string database)
        {
            var h2o_feet = new WaterDepth();

            var query = InfluxQuery.From(h2o_feet).Select(x => new { x.water_level });

            var client = new InfluxQLClient(new Uri(endpoint), database);

            var results = await client.Query(query.Statement);

            double total = 0;
            foreach (var (time, values) in results)
            {
                total += values.water_level;
            }

            return total;
        }

        public async Task<double> SumTheWaterLevelAlt(string endpoint, string database)
        {
            var h2o_feet = new WaterDepth();

            var query = InfluxQuery.From(h2o_feet).Select(x => new { x.water_level });

            var results = await GetPoints(endpoint, database, query.Statement);

            double total = 0;
            foreach (var (time, values) in results)
            {
                total += values.water_level;
            }

            return total;
        }

        private async Task<IEnumerable<(DateTime time, TValues values)>> GetPoints<TValues>(string endpoint, string database, SingleSeriesSelectStatement<TValues> query)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(endpoint) };

            var response = await httpClient.GetAsync($"query?db={Uri.EscapeDataString(database)}&q={Uri.EscapeDataString(query.Text)}");
            response.EnsureSuccessStatusCode();

            var resposeStream = await response.Content.ReadAsStreamAsync();
            var reader = new StreamReader(resposeStream);
            var jsonReader = new Newtonsoft.Json.JsonTextReader(reader);

            return GetPoints<TValues>(jsonReader);
        }

        private IEnumerable<(DateTime time, TValues values)> GetPoints<TValues>(JsonTextReader jsonReader)
        {
            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.StartObject)
            {
                throw new Exception();
            }

            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.PropertyName && "results".Equals(jsonReader.Value))
            {
                throw new Exception();
            }

            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.StartArray)
            {
                throw new Exception();
            }

            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.StartObject)
            {
                throw new Exception();
            }

            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.PropertyName && "statement_id".Equals(jsonReader.Value))
            {
                throw new Exception();
            }

            jsonReader.Skip();

            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.PropertyName && "statement_id".Equals(jsonReader.Value))
            {
                throw new Exception();
            }

            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.StartArray)
            {
                throw new Exception();
            }

            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.StartObject)
            {
                throw new Exception();
            }

            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.PropertyName && "name".Equals(jsonReader.Value))
            {
                throw new Exception();
            }

            jsonReader.Skip();

            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.PropertyName && "columns".Equals(jsonReader.Value))
            {
                throw new Exception();
            }

            jsonReader.Skip();

            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.PropertyName && "values".Equals(jsonReader.Value))
            {
                throw new Exception();
            }

            jsonReader.Read();

            if (jsonReader.TokenType != JsonToken.StartArray)
            {
                throw new Exception();
            }

            while (jsonReader.Read() && jsonReader.TokenType == JsonToken.StartArray)
            {
                var time = jsonReader.ReadAsDateTime().Value;

                var foo = jsonReader.ReadAsDouble().Value;

                var values = (TValues)Activator.CreateInstance(typeof(TValues), foo);

                yield return (time, values);

                jsonReader.Read();

                if (jsonReader.TokenType != JsonToken.EndArray)
                {
                    throw new Exception();
                }
            }

        }

    }
}