using System;
using System.Collections.Generic;

namespace InfluxDB.InfluxQL.Client
{
    public class Series<TValues, TGroupBy>
    {
        public Series( IList<(DateTime time, TValues values)> points, TGroupBy tags)
        {
            Tags = tags;
            Points = points;
        }

        public TGroupBy Tags { get; }

        public IList<(DateTime time, TValues values)> Points { get; }
    }
}