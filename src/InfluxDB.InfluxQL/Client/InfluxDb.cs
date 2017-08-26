using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxDB.InfluxQL.Syntax.Statements;

namespace InfluxDB.InfluxQL.Client
{
    public class InfluxDb
    {
        public Task<IList<(TValues values, DateTime time)>> Query<TValues>(string query)
        {
            return null;
        }

        public Task<IList<Series<TValues, TTags>>> Query<TValues, TTags>(string query)
        {
            return null;
        }

        public Task<IList<(TValues values, DateTime time)>> Query<TValues>(SingleSeriesSelectStatement<TValues> query)
        {
            return Query<TValues>(query.Text);
        }

        public Task<IList<Series<TValues, TTags>>> Query<TValues, TTags>(MultiSeriesSelectStatement<TValues, TTags> query)
        {
            return Query<TValues, TTags>(query.Text);
        }
    }
}