using System;

namespace InfluxDB.InfluxQL.Schema
{
    public class MeasurementField : IEquatable<MeasurementField>
    {
        public MeasurementField(string name) : this(name, name)
        {
        }

        public MeasurementField(string name, string alias)
        {
            InfluxFieldName = name ?? throw new ArgumentNullException(nameof(name));
            DotNetAlias = alias ?? throw new ArgumentNullException(nameof(alias));
        }

        /// <summary>
        /// The measuremnt field name is what the field is called in influx.
        /// </summary>
        public string InfluxFieldName { get; }

        /// <summary>
        /// The alias we use to refer the influx field in dot net (valid C# name).
        /// </summary>
        public string DotNetAlias { get; }

        public bool Equals(MeasurementField other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(InfluxFieldName, other.InfluxFieldName) && string.Equals(DotNetAlias, other.DotNetAlias);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((MeasurementField)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((InfluxFieldName != null ? InfluxFieldName.GetHashCode() : 0) * 397) ^ (DotNetAlias != null ? DotNetAlias.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            if (InfluxFieldName != DotNetAlias)
            {
                return $"{InfluxFieldName} AS {DotNetAlias}";
            }

            return InfluxFieldName;
        }
    }
}