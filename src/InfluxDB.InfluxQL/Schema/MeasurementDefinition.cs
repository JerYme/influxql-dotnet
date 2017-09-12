using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

        public static Measurement<TFields> Define<TFields>(string name, TFields fields)
        {
            return new Measurement<TFields>(name);
        }

        public static TaggedMeasurement<TFields, TTags> Define<TFields, TTags>(string name, TFields fields, TTags tags)
        {
            return new TaggedMeasurement<TFields, TTags>(name);
        }

        protected static IEnumerable<MeasurementField> GetInfluxFields(Type fieldsType)
        {
            return fieldsType.GetTypeInfo().GetProperties().Select(p =>
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

        protected MeasurementField FindField<TFields, T>(Expression<Func<TFields, T>> fieldSection)
        {
            if (fieldSection.Body is MemberExpression member)
            {
                return this.FieldSet.Single(x => x.DotNetAlias == member.Member.Name);
            }

            throw new NotImplementedException();
        }
    }

    public class Measurement<TFields> : Measurement
    {
        public Measurement(string name) : this(name, GetInfluxFields(typeof(TFields)))
        {
        }

        private Measurement(string name, IEnumerable<MeasurementField> fieldSet) : base(name, fieldSet)
        {
        }

        public Measurement<TFields> SetInfluxFieldName<T>(Expression<Func<TFields, T>> fieldSection, string influxFieldName)
        {
            var oldField = this.FindField(fieldSection);
            var newField = new MeasurementField(oldField.DotNetAlias, influxFieldName);

            return new Measurement<TFields>(this.Name, this.FieldSet.Replace(oldField, newField));
        }
    }
}