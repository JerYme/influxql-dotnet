using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using InfluxDB.InfluxQL.Client;
using Newtonsoft.Json;

namespace InfluxDB.InfluxQL.Internal
{
    internal class PointDeserialiser
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly ParameterExpression ReaderParam = Expression.Parameter(typeof(JsonReader), "reader");

        private static readonly Expression ReadTimeExpression = Expression.Call(typeof(PointDeserialiser).GetTypeInfo().GetMethod(nameof(ReadTime), BindingFlags.NonPublic | BindingFlags.Static), ReaderParam);

        private static readonly Dictionary<Type, Expression> ReadValueExpressions = new Dictionary<Type, Expression>
        {
            { typeof(string), Expression.Call(typeof(PointDeserialiser).GetTypeInfo().GetMethod(nameof(ReadString), BindingFlags.NonPublic | BindingFlags.Static), ReaderParam) },
            { typeof(double?), Expression.Call(typeof(PointDeserialiser).GetTypeInfo().GetMethod(nameof(ReadDouble), BindingFlags.NonPublic | BindingFlags.Static), ReaderParam) },
            { typeof(long?), Expression.Call(typeof(PointDeserialiser).GetTypeInfo().GetMethod(nameof(ReadLong), BindingFlags.NonPublic | BindingFlags.Static), ReaderParam) },
            { typeof(bool?), Expression.Call(typeof(PointDeserialiser).GetTypeInfo().GetMethod(nameof(ReadBoolean), BindingFlags.NonPublic | BindingFlags.Static), ReaderParam) }
        };

        static PointDeserialiser()
        {
            var integerTypes = new[] { typeof(long), typeof(int), typeof(uint), typeof(short), typeof(ushort), typeof(byte) };
            foreach (var type in integerTypes)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(type);
                if (nullableType != typeof(long?))
                {
                    ReadValueExpressions[nullableType] = Expression.Convert(ReadValueExpressions[typeof(long?)], nullableType);
                }

                ReadValueExpressions[type] = Expression.Convert(ReadValueExpressions[typeof(long?)], type);
            }

            var floatTypes = new[] { typeof(double), typeof(float), typeof(decimal) };
            foreach (var type in floatTypes)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(type);
                if (nullableType != typeof(double?))
                {
                    ReadValueExpressions[nullableType] = Expression.Convert(ReadValueExpressions[typeof(double?)], nullableType);
                }

                ReadValueExpressions[type] = Expression.Convert(ReadValueExpressions[typeof(double?)], type);
            }

            ReadValueExpressions[typeof(bool)] = Expression.Convert(ReadValueExpressions[typeof(bool?)], typeof(bool));
        }

        public static Func<JsonReader, object> CreateDeserialiser(Type valuesType)
        {
            var createList = CreatePointList(valuesType);
            var pointReader = CreatePointReader(valuesType);

            return reader =>
            {
                var points = createList();

                if (reader.TokenType != JsonToken.StartArray)
                {
                    throw new JsonSerializationException($"Expected start of array. Unexpected token: {reader.TokenType}.");
                }

                while (ReadContent(reader) == JsonToken.StartArray)
                {
                    pointReader(reader, points);

                    ReadArrayEnd(reader);
                }

                switch (reader.TokenType)
                {
                    case JsonToken.EndArray:
                        return points;
                    default:
                        throw new JsonSerializationException($"Expected end of array. Unexpected token: {reader.TokenType}.");
                }
            };
        }

        private static Func<object> CreatePointList(Type valuesType)
        {
            var listType = typeof(List<>).MakeGenericType(typeof(Point<>).MakeGenericType(valuesType));

            return (Func<object>)Expression.Lambda(Expression.Convert(Expression.New(listType), typeof(object))).Compile();
        }

        private static Action<JsonReader, object> CreatePointReader(Type valuesType)
        {
            var pointType = typeof(Point<>).MakeGenericType(valuesType);

            var constructor = pointType.GetTypeInfo().GetConstructor(new[] { typeof(DateTime), valuesType });

            Expression values = CreateValuesReader(valuesType);

            var listType = typeof(List<>).MakeGenericType(typeof(Point<>).MakeGenericType(valuesType));

            var pointsParam = Expression.Parameter(typeof(object), "points");

            var pointsList = Expression.Convert(pointsParam, listType);

            var createPoint = Expression.New(constructor, ReadTimeExpression, values);

            var body = Expression.Call(pointsList, listType.GetTypeInfo().GetMethod("Add", new[] { pointType }), createPoint);

            return (Action<JsonReader, object>)Expression.Lambda(body, ReaderParam, pointsParam).Compile();
        }

        private static Expression CreateValuesReader(Type valuesType)
        {
            var constructor = valuesType.GetTypeInfo().GetConstructors().Single();

            var arguments = constructor.GetParameters().Select(parameter =>
            {
                if (ReadValueExpressions.TryGetValue(parameter.ParameterType, out Expression argument))
                {
                    return argument;
                }

                throw new NotImplementedException($"Type {parameter.ParameterType} is not excepted to be read a value.");
            });

            return Expression.New(constructor, arguments);
        }

        private static JsonToken ReadContent(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.Comment)
                {
                    return reader.TokenType;
                }
            }

            return JsonToken.None;
        }

        private static DateTime ReadTime(JsonReader reader)
        {
            const long NanosecondsPerTick = 100;

            switch (ReadContent(reader))
            {
                case JsonToken.Integer:
                    var time = (long)reader.Value;
                    return UnixEpoch.AddTicks(time / NanosecondsPerTick);
                default:
                    throw new JsonSerializationException($"Error reading time. Unexpected token: {reader.TokenType}.");
            }
        }

        private static string ReadString(JsonReader reader)
        {
            return reader.ReadAsString();
        }

        private static double? ReadDouble(JsonReader reader)
        {
            switch (ReadContent(reader))
            {
                case JsonToken.Float:
                    return (double)reader.Value;
                case JsonToken.Integer:
                    return Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
                case JsonToken.Null:
                    return new double?();
                default:
                    throw new JsonSerializationException($"Expected to read double. Unexpected token: {reader.TokenType}.");
            }
        }

        private static long? ReadLong(JsonReader reader)
        {
            switch (ReadContent(reader))
            {
                case JsonToken.Integer:
                    return (long)reader.Value;
                case JsonToken.Null:
                    return new long?();
                default:
                    throw new JsonSerializationException($"Expected to read long. Unexpected token: {reader.TokenType}.");
            }
        }

        private static bool? ReadBoolean(JsonReader reader)
        {
            switch (ReadContent(reader))
            {
                case JsonToken.Boolean:
                    return (bool)reader.Value;
                case JsonToken.Null:
                    return new bool?();
                default:
                    throw new JsonSerializationException($"Expected to read boolean. Unexpected token: {reader.TokenType}.");
            }
        }

        private static void ReadArrayEnd(JsonReader reader)
        {
            switch (ReadContent(reader))
            {
                case JsonToken.EndArray:
                    return;
                default:
                    throw new JsonSerializationException($"Expected end of array. Unexpected token: {reader.TokenType}.");
            }
        }
    }
}