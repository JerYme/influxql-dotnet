using System;

namespace InfluxDB.InfluxQL.Schema
{
    public class MeasurementTag : IEquatable<MeasurementTag>
    {
        public MeasurementTag(string name) : this(name, name)
        {
        }

        public MeasurementTag(string name, string alias)
        {
            InfluxTagName = name ?? throw new ArgumentNullException(nameof(name));
            DotNetAlias = alias ?? throw new ArgumentNullException(nameof(alias));
        }

        /// <summary>
        /// The measuremnt tag name is what the tag is called in influx.
        /// </summary>
        public string InfluxTagName { get; }

        /// <summary>
        /// The alias we use to refer to the tag in dot net (valid C# name).
        /// </summary>
        public string DotNetAlias { get; }

        public bool Equals(MeasurementTag other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(InfluxTagName, other.InfluxTagName) && string.Equals(DotNetAlias, other.DotNetAlias);
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

            return Equals((MeasurementTag)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((InfluxTagName != null ? InfluxTagName.GetHashCode() : 0) * 397) ^ (DotNetAlias != null ? DotNetAlias.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            if (InfluxTagName != DotNetAlias)
            {
                return $"{InfluxTagName} AS {DotNetAlias}";
            }

            return InfluxTagName;
        }
    }
}