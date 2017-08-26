using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using InfluxDB.InfluxQL.Schema;
using InfluxDB.InfluxQL.Syntax.Literals;

namespace InfluxDB.InfluxQL.Syntax.Clauses
{
    public class GroupByClause
    {
        public GroupByClause(DurationLiteral timeInterval, IEnumerable<MeasurementTag> tags)
        {
            this.TimeInterval = timeInterval;
            this.Tags = tags.ToImmutableList();
        }

        private DurationLiteral TimeInterval { get; }

        public ImmutableList<MeasurementTag> Tags { get; }

        public override string ToString()
        {
            int numverOfGroupings = 0;

            var q = new StringBuilder("GROUP BY ");

            if (TimeInterval != null)
            {
                q.Append("time(").Append(TimeInterval).Append(")");
                numverOfGroupings++;
            }

            foreach (var tag in Tags)
            {
                if (numverOfGroupings > 0)
                {
                    q.Append(",");
                }

                q.Append(tag.InfluxTagName);
                numverOfGroupings++;
            }

            return q.ToString();
        }
    }
}