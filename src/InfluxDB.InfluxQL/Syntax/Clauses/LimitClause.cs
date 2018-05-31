using System;
using System.Globalization;

namespace InfluxDB.InfluxQL.Syntax.Clauses
{
    public class LimitClause
    {
        internal LimitClause(int n)
        {
            if (n < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(n));
            }

            N = n;
        }

        public int N { get; }

        public override string ToString()
        {
            return $"LIMIT {N}";
        }
    }

    public class FillClause
    {
        internal FillClause(double value)
        {
            Value = value;
        }

        internal FillClause(FillType value)
        {
            Type = value;
        }

        public double? Value { get; }
        public FillType Type { get; }

        public override string ToString() => $"fill({Value?.ToString(CultureInfo.InvariantCulture) ?? Type.ToString()})";
    }

    public enum FillType
    {
        /// <summary>
        /// Reports no timestamp and no value for time intervals with no data.
        /// </summary>
        none,
        /// <summary>
        /// Reports null for time intervals with no data but returns a timestamp.This is the same as the default behavior.
        /// </summary>
        @null,
        /// <summary>
        /// Reports the value from the previous time interval for time intervals with no data.
        /// </summary>
        previous,
        /// <summary>
        /// Reports the results of linear interpolation for time intervals with no data.
        /// </summary>
        linear
    }
}