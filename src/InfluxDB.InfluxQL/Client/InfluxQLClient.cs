using System;
using System.Collections.Generic;
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
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly HttpClient httpClient;
        private readonly string database;

        public InfluxQLClient(Uri endpoint, string database, HttpMessageHandler handler = null)
        {
            httpClient = new HttpClient(handler ?? new HttpClientHandler()) { BaseAddress = endpoint };
            this.database = database;
        }

        public async Task<IList<Point<TValues>>> Query<TValues>(SingleSeriesSelectStatement<TValues> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryResponse = await Query(query.Text, cancellationToken);

            var series = queryResponse.Results.Single().Series.Single();

            return GetPoints<TValues>(series).ToList();
        }

        public async Task<IList<Series<TValues, TTags>>> Query<TValues, TTags>(MultiSeriesSelectStatement<TValues, TTags> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryResponse = await Query(query.Text, cancellationToken);

            return queryResponse.Results.Single().Series.Select(serie =>
            {
                TTags tags = (TTags)Activator.CreateInstance(typeof(TTags), query.Tags.Select(t => (object)serie.Tags[t.DotNetAlias]).ToArray());
                var points = GetPoints<TValues>(serie).ToList();
                return new Series<TValues, TTags>(points, tags);
            }).ToList();
        }

        private async Task<QueryResponse> Query(string query, CancellationToken cancellationToken)
        {
            var endpoint = $"query?db={Uri.EscapeDataString(database)}&q={Uri.EscapeDataString(query)}&epoch=ns";

            var response = await httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<QueryResponse>(json);
        }

        private IEnumerable<Point<TValues>> GetPoints<TValues>(QueryResponse.Serie serie)
        {
            const long NanosecondsPerTick = 100;

            // TODO: validate the order of parameters is correct.

            return serie.Values.Select(columnValues =>
            {
                var time = UnixEpoch.AddTicks((long)columnValues[0] / NanosecondsPerTick);
                var values = (TValues)Activator.CreateInstance(typeof(TValues), columnValues.Skip(1).ToArray());

                return new Point<TValues>(time, values);
            });
        }
    }
}