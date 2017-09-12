using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            return tagsType.GetTypeInfo().GetProperties().Select(p =>
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

        protected MeasurementTag FindTag<TTags>(Expression<Func<TTags, string>> fieldSection)
        {
            if (fieldSection.Body is MemberExpression member)
            {
                return this.TagSet.Single(x => x.DotNetAlias == member.Member.Name);
            }

            throw new NotImplementedException();
        }
    }

    public class TaggedMeasurement<TFields, TTags> : TaggedMeasurement
    {
        public TaggedMeasurement(string name) : this(name, GetInfluxFields(typeof(TFields)), GetInfluxTags(typeof(TTags)))
        {
        }

        private TaggedMeasurement(string name, IEnumerable<MeasurementField> fieldSet, IEnumerable<MeasurementTag> tagSet) : base(name, fieldSet, tagSet)
        {
        }

        public TaggedMeasurement<TFields, TTags> SetInfluxFieldName<T>(Expression<Func<TFields, T>> fieldSection, string influxFieldName)
        {
            var oldField = this.FindField(fieldSection);
            var newField = new MeasurementField(oldField.DotNetAlias, influxFieldName);

            return new TaggedMeasurement<TFields, TTags>(this.Name, this.FieldSet.Replace(oldField, newField), TagSet);
        }

        public TaggedMeasurement<TFields, TTags> SetInfluxTagName(Expression<Func<TTags, string>> tagSection, string influxTagName)
        {
            var oldTag = this.FindTag(tagSection);
            var newTag = new MeasurementTag(oldTag.DotNetAlias, influxTagName);

            return new TaggedMeasurement<TFields, TTags>(this.Name, this.FieldSet, TagSet.Replace(oldTag, newTag));
        }
    }
}