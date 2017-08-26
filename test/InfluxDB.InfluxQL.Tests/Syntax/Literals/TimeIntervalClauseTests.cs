using System;
using InfluxDB.InfluxQL.Syntax.Literals;
using Shouldly;
using Xunit;

namespace InfluxDB.InfluxQL.Tests.Syntax.Literals
{
    public class DurationLiteralTests
    {
        [Fact]
        public void Can_use_nanosecond_intervals()
        {
            var interval = new DurationLiteral(TimeSpan.FromTicks(1));

            interval.ToString().ShouldBe("100ns");
        }

        [Fact]
        public void Can_use_microsecond_intervals()
        {
            var interval = new DurationLiteral(TimeSpan.FromTicks(40));

            interval.ToString().ShouldBe("4µ");
        }

        [Fact]
        public void Can_use_millisecond_intervals()
        {
            var interval = new DurationLiteral(TimeSpan.FromMilliseconds(200));

            interval.ToString().ShouldBe("200ms");
        }

        [Fact]
        public void Can_use_second_intervals()
        {
            var interval = new DurationLiteral(TimeSpan.FromSeconds(52));

            interval.ToString().ShouldBe("52s");
        }

        [Fact]
        public void Can_use_minute_intervals()
        {
            var interval = new DurationLiteral(TimeSpan.FromMinutes(17));

            interval.ToString().ShouldBe("17m");
        }

        [Fact]
        public void Can_use_hour_intervals()
        {
            var interval = new DurationLiteral(TimeSpan.FromHours(9));

            interval.ToString().ShouldBe("9h");
        }

        [Fact]
        public void Can_use_day_intervals()
        {
            var interval = new DurationLiteral(TimeSpan.FromDays(3));

            interval.ToString().ShouldBe("3d");
        }

        [Fact]
        public void Can_use_week_intervals()
        {
            var interval = new DurationLiteral(TimeSpan.FromDays(14));

            interval.ToString().ShouldBe("2w");
        }
    }
}