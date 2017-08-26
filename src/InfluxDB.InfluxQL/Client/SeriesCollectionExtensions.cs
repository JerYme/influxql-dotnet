using System;
using System.Collections.Generic;

namespace InfluxDB.InfluxQL.Client
{
    public static class SeriesCollectionExtensions
    {
        public static IList<(TGroupBy tags, TValues values, DateTime time)> Flatten<TValues, TGroupBy>(this IList<Series<TValues, TGroupBy>> series)
        {
            return null;
        }
    }
}