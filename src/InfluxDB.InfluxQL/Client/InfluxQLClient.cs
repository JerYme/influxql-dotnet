using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.InfluxQL.Client.Responses;
using InfluxDB.InfluxQL.Syntax.Statements;
using Newtonsoft.Json;

namespace InfluxDB.InfluxQL.Client
{

    public class InfluxQLClient
    {
        private readonly HttpClient httpClient;
        private readonly string database;
        private readonly JsonSerializer serialiser = JsonSerializer.CreateDefault();

        public InfluxQLClient(Uri endpoint, string database, HttpMessageHandler handler = null)
        {
            httpClient = new HttpClient(handler ?? new HttpClientHandler()) { BaseAddress = endpoint };
            this.database = database;
        }

        public async Task<IList<Point<TValues>>> Query<TValues>(SingleSeriesSelectStatement<TValues> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryResponse = await Query<TValues>(query.Text, cancellationToken);

            var series = queryResponse.Results.Single().Series.Single();

            return series.Values;
        }

        public async Task<IList<Series<TValues, TTags>>> Query<TValues, TTags>(MultiSeriesSelectStatement<TValues, TTags> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryResponse = await Query<TValues>(query.Text, cancellationToken);

            return queryResponse.Results.Single().Series.Select(serie =>
            {
                TTags tags = (TTags)Activator.CreateInstance(typeof(TTags), query.Tags.Select(t => (object)serie.Tags[t.DotNetAlias]).ToArray());
                var points = serie.Values;
                return new Series<TValues, TTags>(points, tags);
            }).ToList();
        }

        private async Task<QueryResponse<TValues>> Query<TValues>(string query, CancellationToken cancellationToken)
        {
            var endpoint = $"query?db={Uri.EscapeDataString(database)}&q={Uri.EscapeDataString(query)}&epoch=ns";

            var get = httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);

            // Ensure that the deserialiser is compiled and cached while the query is being returned.
            PointJsonConverter.GetDeserialiser(typeof(TValues));

            var response = await get;
            response.EnsureSuccessStatusCode();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (var textReader = new StreamReader(responseStream))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                return serialiser.Deserialize<QueryResponse<TValues>>(jsonReader);
            }
        }
    }
}