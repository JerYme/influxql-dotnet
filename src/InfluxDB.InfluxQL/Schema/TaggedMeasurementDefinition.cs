using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace InfluxDB.InfluxQL.Schema
{
    public class TaggedMeasurement : Measurement
    {
        protected TaggedMeasurement(string name, IEnumerable<MeasurementField> fieldSet, IEnumerable<MeasurementTag> tagSet) : base(name, fieldSet)
        {
            TagSet = tagSet.ToImmutableList();
        }

        public ImmutableList<MeasurementTag> TagSet { get; }

        protected static IEnumerable<MeasurementTag> GetInfluxTags(Type tagsType)
        {
            return tagsType.GetProperties().Select(p =>
            {
                var influxFieldName = p.Name;
                var att = p.GetCustomAttributes(typeof(InfluxKeyNameAttribute), false).SingleOrDefault();

                if (att != null)
                {
                    influxFieldName = ((InfluxKeyNameAttribute)att).Name;
                }

                return new MeasurementTag(influxFieldName, p.Name);
            });
        }
    }

    public class TaggedMeasurement<TFields, TTags> : TaggedMeasurement
    {
        public TaggedMeasurement(string name) : base(name, GetInfluxFields(typeof(TFields)), GetInfluxTags(typeof(TTags)))
        {
        }
    }
}