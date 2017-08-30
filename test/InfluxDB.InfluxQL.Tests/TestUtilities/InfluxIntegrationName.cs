using System.Diagnostics;
using System.IO;
using ApprovalTests.Core;

namespace InfluxDB.InfluxQL.Tests.TestUtilities
{
    public class InfluxIntegrationName : IApprovalNamer
    {
        public InfluxIntegrationName(string name)
        {
            Name = name;

            var trace = new StackTrace(true);
            var fileName = trace.GetFrame(0).GetFileName();

            SourcePath = Path.GetDirectoryName(fileName);

            SourcePath = Path.Combine(SourcePath, "../Interactions");
            SourcePath = Path.GetFullPath(SourcePath);
        }

        public string Name { get; }
        public string SourcePath { get; }
    }
}