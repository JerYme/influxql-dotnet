using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InfluxDB.InfluxQL.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests.Client
{
    public class PointJsonConverterTests
    {
        [Theory, MemberData(nameof(TestPoints))]
        public void Can_convert_point(object points)
        {
            var converter = new PointJsonConverter();

            converter.CanConvert(points.GetType()).ShouldBe(true);
        }

        [Theory, MemberData(nameof(TestPoints))]
        public void Serialises_to_json_array_of_arrays(object points)
        {
            var converter = new PointJsonConverter();

            var writer = new JTokenWriter();

            converter.WriteJson(writer, points, JsonSerializer.CreateDefault());

            writer.Token.Type.ShouldBe(JTokenType.Array);
            writer.Token.First.Type.ShouldBe(JTokenType.Array);
        }

        [Theory, MemberData(nameof(TestPoints))]
        public void Can_round_trip(object points)
        {
            var converter = new PointJsonConverter();

            var stringBuilder = new StringBuilder();

            var writer = new JsonTextWriter(new StringWriter(stringBuilder));
            converter.WriteJson(writer, points, JsonSerializer.CreateDefault());

            var reader = new JsonTextReader(new StringReader(stringBuilder.ToString()));
            reader.Read();
            var clone = converter.ReadJson(reader, points.GetType(), null, JsonSerializer.CreateDefault());

            clone.ShouldBe(points);
        }

        [Fact]
        public void Can_deserialsie_mixed_int_and_float_values_to_float()
        {
            var json = "[ [12345, 1.4, 3], [12346, 5, 78.9] ]";

            var reader = new JsonTextReader(new StringReader(json));
            reader.Read();
            var converter = new PointJsonConverter();
            var clone = (IList)converter.ReadJson(reader, typeof(IList<Point<(float, double)>>), null, JsonSerializer.CreateDefault());

            clone.Count.ShouldBe(2);
        }

        public static IEnumerable<object[]> TestPoints()
        {
            List<Point<T>> CreatePoints<T>(params T[] values) => values.Select(x => new Point<T>(DateTime.UtcNow, x)).ToList();

            yield return new object[] { CreatePoints(new { a_double = 34d }) };
            yield return new object[] { CreatePoints(new { a_string = "foo" }) };
            yield return new object[] { CreatePoints(new { a_long = 134L }) };
            yield return new object[] { CreatePoints(new { an_int = 1 }) };
            yield return new object[] { CreatePoints(new { an_bool = true }) };
            yield return new object[] { CreatePoints(new { value = 1 }, new { value = 2 }, new { value = 3 }) };
            yield return new object[] { CreatePoints(new { value = 1d }, new { value = 2.2 }) };
            yield return new object[] { CreatePoints(new { value = (int?)1 }, new { value = (int?)null }) };
            yield return new object[] { CreatePoints((34, "test")) };
        }
    }
}