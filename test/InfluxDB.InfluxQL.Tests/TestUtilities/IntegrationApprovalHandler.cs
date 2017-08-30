using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Core;
using ApprovalTests.Reporters;
using ApprovalTests.Writers;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Shouldly;

namespace InfluxDB.InfluxQL.Tests.TestUtilities
{
    public class IntegrationApprovalHandler : HttpMessageHandler
    {
        private readonly IApprovalNamer integrationName;
        private readonly StringWriter interactions;
        private readonly JsonTextWriter interactionWriter;

        public IntegrationApprovalHandler( IApprovalNamer integrationName)
        {
            this.integrationName = integrationName;
            this.interactions = new StringWriter();

            this.interactionWriter = new JsonTextWriter(interactions)
            {
                Formatting = Formatting.Indented
            };

            this.interactionWriter.WriteStartArray();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Method.ShouldBe(HttpMethod.Get);
            request.Content.ShouldBeNull();

            var influx = new HttpClient();
            var influxRequest = new HttpRequestMessage(request.Method, request.RequestUri);
            var influxResponse = await influx.SendAsync(influxRequest, cancellationToken);


            interactionWriter.WriteStartObject();
            interactionWriter.WritePropertyName("request");
            WriteRequest(request);

            interactionWriter.WritePropertyName("response");
            await WriteResponse(influxResponse);
            interactionWriter.WriteEndObject();

            return influxResponse;
        }


        private void WriteRequest(HttpRequestMessage request)
        {
            var queryParameters = new SortedDictionary<string, string>(QueryHelpers.ParseQuery(request.RequestUri.Query).ToDictionary(x => x.Key, x => x.Value.ToString()));

            interactionWriter.WriteStartObject();
            interactionWriter.WritePropertyName("queryParameters");
            new JsonSerializer().Serialize(interactionWriter, queryParameters);
            interactionWriter.WriteEndObject();
        }

        private async Task WriteResponse(HttpResponseMessage response)
        {
            interactionWriter.WriteStartObject();
            interactionWriter.WritePropertyName("status");
            interactionWriter.WriteValue(response.StatusCode);

            var influxJson = await response.Content.ReadAsStringAsync();

            interactionWriter.WritePropertyName("content");
            interactionWriter.WriteToken(new JsonTextReader(new StringReader(influxJson)));
            interactionWriter.WriteEndObject();
        }

        public void Verify()
        {
            this.interactionWriter.WriteEndArray();

            var json = interactions.ToString();
            var writer = WriterFactory.CreateTextWriter(json, nameof(json));

            Approvals.Verify(writer, integrationName, new DiffReporter());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Verify();
            }

            base.Dispose(disposing);
        }
    }
}