using System;

namespace InfluxDB.InfluxQL.Syntax.Clauses
{
    public class SLimitClause
    {
        internal SLimitClause(int n)
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
            return $"SLIMIT {N}";
        }
    }
}