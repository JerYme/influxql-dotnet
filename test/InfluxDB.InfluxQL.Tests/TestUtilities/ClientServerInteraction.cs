using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfluxDB.InfluxQL.Tests.TestUtilities
{
    public class ClientServerInteraction
    {
        public ClientServerInteraction(RequestInfo request, ResponseInfo response)
        {
            Request = request;
            Response = response;
        }

        public RequestInfo Request { get; }

        public ResponseInfo Response { get; }

        public class RequestInfo
        {
            public RequestInfo(IDictionary<string, string> queryParameters)
            {
                QueryParameters = new SortedDictionary<string, StringValues>(queryParameters.ToDictionary(x => x.Key, x => new StringValues(x.Value)));
            }

            public IDictionary<string, StringValues> QueryParameters { get; }
        }

        public class ResponseInfo
        {
            public ResponseInfo(int status, JObject content)
            {
                Status = status;
                Content = FormatJsonHowInfluxDoesIt(content);
            }

            public int Status { get; }
            public string Content { get; }

            private static string FormatJsonHowInfluxDoesIt(JObject json)
            {
                var formatted = new StringWriter();
                var writer = new JsonTextWriter(formatted);

                json.WriteTo(writer);

                formatted.Write('\n'); // Influx json responses always end in a newline char.

                return formatted.ToString();
            }
        }
    }
}