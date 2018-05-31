using System;
using System.Collections.Generic;

namespace InfluxDB.InfluxQL.Client
{
    public static class SeriesCollectionExtensions
    {
        //public static IEnumerable<(DateTime time, TValues values, TGroupBy tags)> Flatten<TValues, TGroupBy>(this IList<Series<TValues, TGroupBy>> series)
        //{
        //    foreach (var serie in series)
        //    {
        //        foreach (var point in serie.Points)
        //        {
        //            yield return (point.Time, point.Values, serie.Tags);
        //        }
        //    }
        //}
    }
}