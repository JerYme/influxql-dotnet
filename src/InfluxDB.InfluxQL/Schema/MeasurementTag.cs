namespace InfluxDB.InfluxQL.Schema
{
    public class MeasurementTag
    {
        public MeasurementTag(string name, string alias)
        {
            InfluxTagName = name;
            DotNetAlias = alias;
        }

        /// <summary>
        /// The measuremnt tag name is what the tag is called in influx.
        /// </summary>
        public string InfluxTagName { get; }

        /// <summary>
        /// The alias we use to refer to the tag in dot net (valid C# name).
        /// </summary>
        public string DotNetAlias { get; }
    }
}