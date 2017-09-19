using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using InfluxDB.InfluxQL.Internal;
using Newtonsoft.Json;

namespace InfluxDB.InfluxQL.Client
{
    public class PointJsonConverter : JsonConverter
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly IDictionary<Type, Action<JsonWriter, object>> Serialisers = new Dictionary<Type, Action<JsonWriter, object>>();

        private static readonly IDictionary<Type, Func<JsonReader, object>> Deserialisers = new Dictionary<Type, Func<JsonReader, object>>();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var valuesType = GetValuesType(value.GetType());

            if (!Serialisers.TryGetValue(valuesType, out Action<JsonWriter, object> serialiserAction))
            {
                var writerParam = Expression.Parameter(typeof(JsonWriter), "writer");
                var valueParam = Expression.Parameter(typeof(object), "value");

                var writeJsonMethod = this.GetType().GetTypeInfo().GetMethod(nameof(WriteJson), BindingFlags.NonPublic | BindingFlags.Static);

                var call = Expression.Call(writeJsonMethod.MakeGenericMethod(valuesType), writerParam, valueParam);

                serialiserAction = (Action<JsonWriter, object>)Expression.Lambda(call, writerParam, valueParam).Compile();

                Serialisers.Add(valuesType, serialiserAction);
            }

            serialiserAction(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            var valuesType = GetValuesType(objectType);

            var deserialiserFunc = GetDeserialiser(valuesType);

            return deserialiserFunc(reader);
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == null)
            {
                return false;
            }

            return GetValuesType(objectType) != null;
        }

        public Type GetValuesType(Type objectType)
        {
            var objectTypeInfo = objectType.GetTypeInfo();

            var enumerables = objectTypeInfo.GetInterfaces().Select(x => x.GetTypeInfo()).Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            foreach (var enumerableInterface in enumerables)
            {
                var enumeratingType = enumerableInterface.GenericTypeArguments[0].GetTypeInfo();

                if (enumeratingType.IsGenericType && enumeratingType.GetGenericTypeDefinition() == typeof(Point<>))
                {
                    return enumeratingType.GenericTypeArguments[0];
                }
            }

            return null;
        }

        internal static Func<JsonReader, object> GetDeserialiser(Type valuesType)
        {
            if (valuesType == null)
            {
                throw new ArgumentNullException(nameof(valuesType));
            }

            if (!Deserialisers.TryGetValue(valuesType, out Func<JsonReader, object> deserialiserFunc))
            {
                Deserialisers.Add(valuesType, deserialiserFunc = PointDeserialiser.CreateDeserialiser(valuesType));
            }

            return deserialiserFunc;
        }

        private static void WriteJson<TValues>(JsonWriter writer, object value)
        {
            var points = (IEnumerable<Point<TValues>>)value;

            writer.WriteStartArray();

            foreach (var point in points)
            {
                writer.WriteStartArray();

                writer.WriteValue((point.Time - UnixEpoch).Ticks * 100);

                foreach (var member in typeof(TValues).GetTypeInfo().GetMembers())
                {
                    switch (member)
                    {
                        case PropertyInfo property:
                            writer.WriteValue(property.GetValue(point.Values));
                            break;
                        case FieldInfo field:
                            writer.WriteValue(field.GetValue(point.Values));
                            break;
                    }
                }

                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }
    }
}