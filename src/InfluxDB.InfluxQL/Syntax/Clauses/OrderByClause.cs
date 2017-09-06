namespace InfluxDB.InfluxQL.Syntax.Clauses
{
    public class OrderByClause
    {
        internal OrderByClause()
        {
        }

        public override string ToString()
        {
            return "ORDER BY time DESC";
        }
    }
}