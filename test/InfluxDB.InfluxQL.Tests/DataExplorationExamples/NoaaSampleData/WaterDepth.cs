using InfluxDB.InfluxQL.Schema;

namespace InfluxDB.InfluxQL.Tests.DataExplorationExamples.NoaaSampleData
{
    public sealed class WaterDepth : TaggedMeasurement<WaterDepth.Fields, WaterDepth.Tags>
    {
        public WaterDepth() : base("h2o_feet")
        {
        }

        public sealed class Tags
        {
            public string location { get; }
        }

        public sealed class Fields
        {
            [InfluxKeyName("level description")]
            public string level_description { get; set; }

            public double water_level { get; set; }
        }
    }
}

