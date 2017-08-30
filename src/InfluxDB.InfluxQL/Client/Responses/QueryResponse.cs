using System.Collections.Generic;

namespace InfluxDB.InfluxQL.Client.Responses
{
    internal class QueryResponse
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

            public IList<IList<object>> Values { get; set; }
        }
    }
}