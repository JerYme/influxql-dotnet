using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace InfluxDB.InfluxQL.Schema
{
    public class Measurement
    {
        protected Measurement(string name, IEnumerable<MeasurementField> fieldSet)
        {
            Name = name;
            FieldSet = fieldSet.ToImmutableList();
        }

        public string Name { get; }

        public ImmutableList<MeasurementField> FieldSet { get; }

        protected static IEnumerable<MeasurementField> GetInfluxFields(Type fieldsType)
        {
            return fieldsType.GetProperties().Select(p =>
            {
                var influxFieldName = p.Name;
                var att = p.GetCustomAttributes(typeof(InfluxKeyNameAttribute), false).SingleOrDefault();

                if (att != null)
                {
                    influxFieldName = ((InfluxKeyNameAttribute)att).Name;
                }

                return new MeasurementField(influxFieldName, p.Name);
            });
        }
    }

    public class Measurement<TFields> : Measurement
    {
        public Measurement(string name) : base(name, GetInfluxFields(typeof(TFields)))
        {
        }
    }
}