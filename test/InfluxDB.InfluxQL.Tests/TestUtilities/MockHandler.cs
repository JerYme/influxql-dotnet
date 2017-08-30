using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApprovalTests.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;

namespace InfluxDB.InfluxQL.Tests.TestUtilities
{
    public class MockHandler : HttpMessageHandler
    {
        private readonly IEnumerator<ClientServerInteraction> senario;

        public MockHandler(string name)
        {
            var stream = GetType().Assembly.GetManifestResourceStream($"InfluxDB.InfluxQL.Tests.Interactions.{name}.approved.json");
            var interactionJson = new JsonTextReader(new StreamReader(stream));

            var serialiser = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var interaction = serialiser.Deserialize<ClientServerInteraction[]>(interactionJson);
            this.senario = interaction.Cast<ClientServerInteraction>().GetEnumerator();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ShouldBeTestExtensions.ShouldBe(senario.MoveNext(), true);

            request.Method.ShouldBe(HttpMethod.Get);
            request.Content.ShouldBeNull();

            var queryParameters = new SortedDictionary<string, StringValues>(QueryHelpers.ParseQuery(request.RequestUri.Query));
            var expectedQueryParameters = new SortedDictionary<string, StringValues>(senario.Current.Request.QueryParameters);

            queryParameters.ShouldBe(expectedQueryParameters);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(senario.Current.Response.Content, Encoding.UTF8, "application/json")
            };

            return response;
        }
    }
}