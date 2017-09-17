using System;

namespace InfluxDB.InfluxQL.Client
{
    public struct Point<TValues>
    {
        public Point(DateTime time, TValues values)
        {
            Time = time;
            Values = values;
        }

        public DateTime Time { get; }

        public TValues Values { get; }

        public void Deconstruct(out DateTime time, out TValues values)
        {
            time = Time;
            values = Values;
        }
    }
}