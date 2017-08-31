using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using ApprovalTests.Core;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace InfluxDB.InfluxQL.Tests.TestUtilities
{
    public class InfluxDataAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { new MockConfig(testMethod.Name) };
#if DEBUG
            // TODO: Categorise these tests as integration tests so they can be run seperatly on build server.
            var integrationName = new InfluxIntegrationName(testMethod.Name);
            yield return new object[] { new IntegrationConfig("http://localhost:8086", testMethod.Name, integrationName.SourcePath) };
#endif
        }
    }

    public interface IInfluxTestConfig
    {
        Uri Endpoint { get; }
        HttpMessageHandler CreateHandler();
    }

    public class IntegrationConfig : IXunitSerializable, IInfluxTestConfig, IApprovalNamer
    {
        private string endpoint;

        public IntegrationConfig()
        {
        }

        public IntegrationConfig(string endpoint, string name, string sourcePath)
        {
            this.endpoint = endpoint;
            Name = name;
            SourcePath = sourcePath;
        }

        public string Endpoint => endpoint;

        Uri IInfluxTestConfig.Endpoint => new Uri(endpoint);

        protected string Name { get; private set; }

        protected string SourcePath { get; private set; }

        string IApprovalNamer.Name => Name;

        string IApprovalNamer.SourcePath => SourcePath;

        public void Deserialize(IXunitSerializationInfo info)
        {
            endpoint = info.GetValue<string>(nameof(endpoint));
            Name = info.GetValue<string>("name");
            SourcePath = info.GetValue<string>("path");
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(endpoint), endpoint);
            info.AddValue("name", Name);
            info.AddValue("path", SourcePath);
        }

        public HttpMessageHandler CreateHandler()
        {
            return new IntegrationApprovalHandler(this);
        }
    }

    public class MockConfig : IXunitSerializable, IInfluxTestConfig
    {
        private string name;

        public MockConfig()
        {
        }

        public MockConfig(string name)
        {
            this.name = name;
        }

        Uri IInfluxTestConfig.Endpoint => new Uri("http://localhost:8086");

        public void Deserialize(IXunitSerializationInfo info)
        {
            name = info.GetValue<string>(nameof(name));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(name), name);
        }

        public HttpMessageHandler CreateHandler()
        {
            return new MockHandler(name);
        }
    }
}