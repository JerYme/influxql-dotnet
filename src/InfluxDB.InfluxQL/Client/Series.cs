using System.Collections.Generic;

namespace InfluxDB.InfluxQL.Client
{
    public class Series<TValues, TGroupBy>
    {
        public Series( IList<Point<TValues>> points, TGroupBy tags)
        {
            Tags = tags;
            Points = points;
        }

        public TGroupBy Tags { get; }

        public IList<Point<TValues>> Points { get; }
    }
}