using System;
using System.Collections.Generic;

namespace InfluxDB.InfluxQL.Client
{
    public class Series<TValues, TGroupBy>
    {
        public TGroupBy Tags { get; }

        public IList<(TValues values, DateTime time)> Points { get; }

    }
}