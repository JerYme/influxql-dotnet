namespace InfluxDB.InfluxQL.Schema
{
    public class MeasurementField
    {
        public MeasurementField(string name, string alias)
        {
            InfluxFieldName = name;
            DotNetAlias = alias;
        }

        /// <summary>
        /// The measuremnt field name is what the field is called in influx.
        /// </summary>
        public string InfluxFieldName { get; }

        /// <summary>
        /// The alias we use to refer the influx field in dot net (valid C# name).
        /// </summary>
        public string DotNetAlias { get; }
    }
}