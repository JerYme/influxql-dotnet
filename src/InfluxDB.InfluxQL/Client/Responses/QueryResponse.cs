using System.Collections.Generic;
using Newtonsoft.Json;

namespace InfluxDB.InfluxQL.Client.Responses
{
    internal class QueryResponse<TValues>
    {
        public IEnumerable<SeriesResult> Results { get; set; }

        public class SeriesResult
        {
            public IEnumerable<Serie> Series { get; set; }
        }

        public class Serie
        {
            public string Name { get; set; }

            public IDictionary<string, string> Tags { get; set; }

            public IList<string> Columns { get; set; }

            [JsonConverter(typeof(PointJsonConverter))]
            public IList<Point<TValues>> Values { get; set; }
        }
    }
}