using System;

namespace InfluxDB.InfluxQL.Syntax.Literals
{
    /// <summary>
    /// https://docs.influxdata.com/influxdb/v1.3/query_language/spec/#durations
    /// </summary>
    public class DurationLiteral
    {
        private readonly TimeSpan timeInterval;

        public DurationLiteral(TimeSpan timeInterval)
        {
            this.timeInterval = timeInterval;
        }

        public override string ToString()
        {
            if (timeInterval.Ticks % TimeSpan.TicksPerDay == 0)
            {
                if (timeInterval.Days % 7 == 0)
                {
                    return $"{timeInterval.Days / 7}w";
                }

                return $"{timeInterval.Days}d";
            }

            if (timeInterval.Ticks % TimeSpan.TicksPerHour == 0)
            {
                return $"{timeInterval.Hours}h";
            }

            if (timeInterval.Ticks % TimeSpan.TicksPerMinute == 0)
            {
                return $"{timeInterval.Minutes}m";
            }

            if (timeInterval.Ticks % TimeSpan.TicksPerSecond == 0)
            {
                return $"{timeInterval.Seconds}s";
            }

            if (timeInterval.Ticks % TimeSpan.TicksPerMillisecond == 0)
            {
                return $"{timeInterval.Milliseconds}ms";
            }

            if (timeInterval.Ticks % 10 == 0)
            {
                return $"{timeInterval.Ticks / 10}µ";
            }

            return $"{timeInterval.Ticks * 100}ns";
        }
    }
}