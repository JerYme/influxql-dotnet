using InfluxDB.InfluxQL.Schema;

namespace InfluxDB.InfluxQL.Tests.DataExplorationExamples.NoaaSampleData
{
    public sealed class WaterDepth : TaggedMeasurement<WaterDepth.Fields, WaterDepth.Tags>
    {
        public WaterDepth() : base("h2o_feet")
        {
        }

        public struct Tags
        {
            public Tags(string location)
            {
                this.location = location;
            }

            public string location { get; }
        }

        public struct Fields
        {
            public Fields(string levelDescription, double waterLevel)
            {
                level_description = levelDescription;
                water_level = waterLevel;
            }

            [InfluxKeyName("level description")]
            public string level_description { get; }

            public double water_level { get; }
        }
    }
}

